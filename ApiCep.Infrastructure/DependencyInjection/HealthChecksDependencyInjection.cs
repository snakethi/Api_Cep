using ApiCep.Infrastructure.Data;
using ApiCep.Infrastructure.ExternalServices;
using ApiCep.Infrastructure.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ApiCep.Infrastructure
{
    internal static class HealthChecksDependencyInjection
    {
        internal static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services)
        {
            services.AddHttpClient(ViaCepHealthCheck.HttpClientName, (serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ViaCepSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(3);
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("A API está em execução."), tags: ["live"])
                .AddDbContextCheck<ApplicationDbContext>("sqlserver", tags: ["ready"])
                .AddCheck<ViaCepHealthCheck>("viacep", tags: ["ready"]);

            return services;
        }
    }
}
