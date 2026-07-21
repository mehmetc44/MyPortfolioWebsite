using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Data;
using Server.Health;
using Server.Services;

namespace Server.Extensions
{
    public static class ServiceExtensions
    {
        public static void LoadEnvironmentVariablesAndEnsureFolders()
        {
            // Load environment variables from .env
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "../.env");
            if (File.Exists(envPath))
            {
                foreach (var line in File.ReadAllLines(envPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#")) continue;
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var val = parts[1].Trim();
                        Environment.SetEnvironmentVariable(key, val);
                    }
                }
            }

            // Ensure database directory exists
            var dbFolder = Path.Combine(Directory.GetCurrentDirectory(), "Db");
            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database connection string setup
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration["DB_CONNECTION_STRING"]));

            // HttpClient registration for external API integrations (Supabase Storage, Email APIs)
            services.AddHttpClient();

            // File and email services (Using Supabase Storage Service)
            services.AddScoped<IFileService, SupabaseStorageService>();
            services.AddScoped<IEmailService, EmailService>();

            // Database seeding service
            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

            // Configure Simple Token Authentication (for cross-site CORS robustness)
            services.AddAuthentication("SimpleToken")
                .AddScheme<SimpleTokenAuthOptions, SimpleTokenAuthHandler>("SimpleToken", null);

            // CORS policy config with allowed credentials restricted to localhost and custom production domains
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.SetIsOriginAllowed(origin => 
                    {
                        var host = new Uri(origin).Host;
                        return host == "localhost" || 
                               host == "127.0.0.1" || 
                               host == "www.mehmetcakmak.online" || 
                               host == "mehmetcakmak.online";
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            // Health check middleware configuration (pings Supabase Postgres DB & Storage REST API)
            services.AddHealthChecks()
                .AddCheck<Server.Health.SupabaseHealthCheck>("supabase");

            return services;
        }

        public static void SeedDatabase(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
                seeder.Seed();
            }
        }
    }
}
