using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using ApiCep.Infrastructure.Authentication;
using ApiCep.Infrastructure;

namespace ApiCep.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        private const string CorsPolicyName = "PermitirTudo";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddInfrastructure(_configuration);

            ConfigurarJwt(services);
            ConfigurarSwagger(services);
        }

        public void Configure(WebApplication app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_CEP");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseCors(CorsPolicyName);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }


        private void ConfigurarJwt(IServiceCollection services)
        {
            var jwtSettings = _configuration.GetRequiredSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("As configurações JWT não foram encontradas.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
                string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
                string.IsNullOrWhiteSpace(jwtSettings.Audience) ||
                jwtSettings.ExpireMinutes <= 0)
            {
                throw new InvalidOperationException("As configurações JWT são inválidas.");
            }

            services.Configure<JwtSettings>(
                _configuration.GetRequiredSection(JwtSettings.SectionName));

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(key),

                            ValidateIssuer = true,
                            ValidIssuer = jwtSettings.Issuer,

                            ValidateAudience = true,
                            ValidAudience = jwtSettings.Audience,

                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                });

            services.AddAuthorization();
        }

        private void ConfigurarSwagger(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ApiCep API",
                    Version = "v1",
                    Description = "API para gerenciamento de usuários, endereços e consultas ao ViaCEP."
                });

                options.AddSecurityDefinition(
                    "bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Informe o token JWT obtido no endpoint de login."
                    });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecuritySchemeReference(
                                "bearer",
                                document)
                        ] = new List<string>()
                    });
            });
        }

    }
}
