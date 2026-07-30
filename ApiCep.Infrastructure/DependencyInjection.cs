using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ApiCep.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.AddSecurity(configuration);
            services.AddExports();
            services.AddViaCep(configuration);
            services.AddInfrastructureHealthChecks();

            return services;
        }
    }
}
