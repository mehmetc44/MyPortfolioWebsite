using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Server.Data;

namespace Server.Health
{
    public class SupabaseHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;

        public SupabaseHealthCheck(AppDbContext dbContext, HttpClient httpClient, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
            _supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") 
                           ?? configuration["SUPABASE_URL"] 
                           ?? configuration["Supabase:Url"] 
                           ?? "";
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Ping Supabase PostgreSQL Database via EF Core query
                bool canConnectDb = await _dbContext.Database.CanConnectAsync(cancellationToken);

                // 2. Ping Supabase Storage REST API endpoint to keep project active
                bool canConnectStorage = true;
                if (!string.IsNullOrWhiteSpace(_supabaseUrl))
                {
                    try
                    {
                        var healthUrl = $"{_supabaseUrl.TrimEnd('/')}/storage/v1/health";
                        using var request = new HttpRequestMessage(HttpMethod.Get, healthUrl);
                        var response = await _httpClient.SendAsync(request, cancellationToken);
                        canConnectStorage = response.IsSuccessStatusCode;
                    }
                    catch
                    {
                        canConnectStorage = false;
                    }
                }

                if (canConnectDb && canConnectStorage)
                {
                    return HealthCheckResult.Healthy("Supabase Database and Storage services are active.");
                }
                else if (canConnectDb)
                {
                    return HealthCheckResult.Degraded("Supabase Database is healthy, Storage ping pending.");
                }

                return HealthCheckResult.Unhealthy("Failed to ping Supabase services.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Supabase health check error: {ex.Message}");
            }
        }
    }
}
