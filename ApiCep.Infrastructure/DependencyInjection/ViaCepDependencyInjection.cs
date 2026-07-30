using ApiCep.Application.Interfaces.ExternalServices;
using ApiCep.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace ApiCep.Infrastructure
{
    internal static class ViaCepDependencyInjection
    {
        internal static IServiceCollection AddViaCep(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ViaCepSettings>()
                .Bind(configuration.GetSection(ViaCepSettings.SectionName))
                .Validate(settings => Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _), "A URL base do ViaCEP é inválida.")
                .ValidateOnStart();

            services.AddMemoryCache();

            services.AddHttpClient<IViaCepService, ViaCepService>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ViaCepSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            })
            .AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
                options.Retry.MaxRetryAttempts = 2;
            });

            return services;
        }
    }
}
