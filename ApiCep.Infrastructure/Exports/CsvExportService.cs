using ApiCep.Application.Exports.Models;
using ApiCep.Application.Interfaces.FileExport;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace ApiCep.Infrastructure.Exports
{
    public sealed class CsvExportService : ICsvExportService
    {
        public byte[] GenerateUsersWithAddresses(IReadOnlyCollection<UserAddressCsvRow> rows)
        {
            ArgumentNullException.ThrowIfNull(rows);

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true), 1024, true);
            using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            });

            csvWriter.WriteRecords(rows);
            streamWriter.Flush();

            return memoryStream.ToArray();
        }
    }
}
