using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AddressEntity = ApiCep.Domain.Entities.Address;

namespace ApiCep.Infrastructure.Repositories
{
    public sealed class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<AddressEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAtUtc == null && x.User.DeletedAtUtc == null, cancellationToken);
        }

        public async Task<IReadOnlyCollection<AddressEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Addresses.AsNoTracking()
                .Where(x => x.UserId == userId && x.DeletedAtUtc == null && x.User.DeletedAtUtc == null)
                .OrderBy(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(AddressEntity address, CancellationToken cancellationToken = default)
        {
            await _context.Addresses.AddAsync(address, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
