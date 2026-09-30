using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Server.Extensions;

// 1. Initialize environment variables and verify/create directories
ServiceExtensions.LoadEnvironmentVariablesAndEnsureFolders();

// Prevent inotify limit issues in containerized environments (like Docker/Render/Kubernetes)
System.Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");

var builder = WebApplication.CreateBuilder(args);

// 2. Add services (DbContext, custom services, CORS, Health Checks)
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Angular/TypeScript tarafı camelCase bekler (category_DE vs Category_DE)
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Configure HTTP pipeline
app.UseStaticFiles();

// 4. Migrate database and run data seeding
app.SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 5. Map the health check middleware endpoint
app.MapHealthChecks("/health");

app.Run();
