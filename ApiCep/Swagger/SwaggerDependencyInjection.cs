using Microsoft.OpenApi;

namespace ApiCep.Api.Swagger
{

    public static class SwaggerDependencyInjection
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ApiCep API",
                    Version = "v1",
                    Description = "API para gerenciamento de usuários, endereços e consultas ao ViaCEP."
                });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Informe somente o token JWT obtido no endpoint de login, sem escrever Bearer."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = new List<string>()
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiCep API v1");
                options.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}
