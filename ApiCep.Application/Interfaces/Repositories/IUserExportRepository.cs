using ApiCep.Application.Exports.Models;

namespace ApiCep.Application.Interfaces.Repositories
{
    public interface IUserExportRepository
    {
        Task<IReadOnlyCollection<UserAddressCsvRow>> GetUsersWithAddressesAsync(Guid? userId, CancellationToken cancellationToken = default);
    }
}
