using ApiCep.Application.Exports.Models;

namespace ApiCep.Application.Interfaces.FileExport
{
    public interface ICsvExportService
    {
        byte[] GenerateUsersWithAddresses(IReadOnlyCollection<UserAddressCsvRow> rows);
    }
}
