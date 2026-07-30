using ApiCep.Application.Interfaces.Security;
using ApiCep.Infrastructure.Authentication;
using ApiCep.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;


namespace ApiCep.Infrastructure
{
    internal static class SecurityDependencyInjection
    {
        internal static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>()
                .Bind(configuration.GetSection(JwtSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Key), "A chave JWT é obrigatória.")
                .Validate(settings => Encoding.UTF8.GetByteCount(settings.Key) >= 32, "A chave JWT deve possuir pelo menos 32 bytes.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Issuer), "O emissor JWT é obrigatório.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.Audience), "A audiência JWT é obrigatória.")
                .Validate(settings => settings.ExpireMinutes > 0, "O tempo de expiração JWT deve ser maior que zero.")
                .ValidateOnStart();

            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IAccessTokenService, JwtAccessTokenService>();

            return services;
        }
    }
}
