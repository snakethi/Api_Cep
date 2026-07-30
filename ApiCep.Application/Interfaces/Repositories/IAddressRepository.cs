using AddressEntity = ApiCep.Domain.Entities.Address;

namespace ApiCep.Application.Interfaces.Repositories
{
    public interface IAddressRepository
    {
        Task<AddressEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<AddressEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(AddressEntity address, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
