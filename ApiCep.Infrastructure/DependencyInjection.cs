using ApiCep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCep.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("A connection string DefaultConnection não foi configurada.");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
