using ApiCep.Api.Authentication;
using ApiCep.Api.ExceptionHandling;
using ApiCep.Api.HealthChecks;
using ApiCep.Api.RateLimiting;
using ApiCep.Api.Swagger;
using ApiCep.Api.Versioning;
using ApiCep.Application;
using ApiCep.Infrastructure;

namespace ApiCep.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddApplication();
            services.AddInfrastructure(_configuration);

            services.AddApiExceptionHandling();
            services.AddJwtAuthentication(_configuration);
            services.AddApiVersioningConfiguration();
            services.AddSwaggerDocumentation();
            services.AddApiRateLimiting();
        }

        public void Configure(WebApplication app)
        {
            app.UseExceptionHandler();

            if (_environment.IsDevelopment())
                app.UseSwaggerDocumentation();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();
            app.MapControllers();
            app.MapApiHealthChecks();
        }
    }
}
