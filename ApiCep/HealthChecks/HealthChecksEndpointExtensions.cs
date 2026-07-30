using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiCep.Api.HealthChecks
{
    public static class HealthChecksEndpointExtensions
    {
        public static WebApplication MapApiHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("live"),
                ResponseWriter = WriteResponseAsync
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
                ResponseWriter = WriteResponseAsync
            });

            return app;
        }

        private static Task WriteResponseAsync(HttpContext httpContext, HealthReport healthReport)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                status = healthReport.Status.ToString(),
                totalDurationMilliseconds = healthReport.TotalDuration.TotalMilliseconds,
                checks = healthReport.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMilliseconds = entry.Value.Duration.TotalMilliseconds
                })
            };

            return httpContext.Response.WriteAsJsonAsync(response, httpContext.RequestAborted);
        }
    }
}
