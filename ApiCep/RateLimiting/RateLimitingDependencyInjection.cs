using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ApiCep.Api.RateLimiting
{
    public static class RateLimitingDependencyInjection
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);

                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Muitas requisições",
                        Detail = "O limite de requisições foi excedido. Aguarde alguns instantes e tente novamente.",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                };

                options.AddPolicy<string>(RateLimitPolicies.Api, httpContext =>
                {
                    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var partitionKey = userId ?? ipAddress;

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
                });

                options.AddPolicy<string>(RateLimitPolicies.Login, httpContext =>
                {
                    var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
                });
            });

            return services;
        }
    }
}
