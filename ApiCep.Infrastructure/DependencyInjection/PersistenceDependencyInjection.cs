using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Infrastructure.Data;
using ApiCep.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCep.Infrastructure
{
    internal static class PersistenceDependencyInjection
    {
        internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("A connection string DefaultConnection não foi configurada.");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IUserExportRepository, UserExportRepository>();

            return services;
        }
    }
}
