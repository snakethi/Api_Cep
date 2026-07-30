using ApiCep.Application.Interfaces.FileExport;
using ApiCep.Infrastructure.Exports;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCep.Infrastructure
{
    internal static class ExportDependencyInjection
    {
        internal static IServiceCollection AddExports(this IServiceCollection services)
        {
            services.AddScoped<ICsvExportService, CsvExportService>();

            return services;
        }
    }
}
