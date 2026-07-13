using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Extensions;

// 1. Initialize environment variables and verify/create directories
ServiceExtensions.LoadEnvironmentVariablesAndEnsureFolders();

var builder = WebApplication.CreateBuilder(args);

// 2. Add services (DbContext, custom services, CORS, Health Checks)
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Configure HTTP pipeline
app.UseStaticFiles();

// 4. Migrate database and run data seeding
app.SeedDatabase();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 5. Map the health check middleware endpoint
app.MapHealthChecks("/health");

app.Run();
