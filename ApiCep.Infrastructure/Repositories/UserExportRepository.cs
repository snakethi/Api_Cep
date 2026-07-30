using ApiCep.Application.Exports.Models;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;



namespace ApiCep.Infrastructure.Repositories
{
    public sealed class UserExportRepository : IUserExportRepository
    {
        private readonly ApplicationDbContext _context;

        public UserExportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<UserAddressCsvRow>> GetUsersWithAddressesAsync(Guid? userId, CancellationToken cancellationToken = default)
        {
            var users = _context.Users
                .AsNoTracking()
                .Where(user => user.DeletedAtUtc == null);

            if (userId.HasValue)
                users = users.Where(user => user.Id == userId.Value);

            var query =
                from user in users
                join address in _context.Addresses.AsNoTracking().Where(address => address.DeletedAtUtc == null)
                    on user.Id equals address.UserId into addresses
                from address in addresses.DefaultIfEmpty()
                orderby user.Name, address!.CreatedAtUtc
                select new UserAddressCsvRow(
                    user.Id,
                    user.Name,
                    user.Email,
                    user.IsActive,
                    address == null ? null : address.Id,
                    address == null ? null : address.ZipCode,
                    address == null ? null : address.Street,
                    address == null ? null : address.Number,
                    address == null ? null : address.Neighborhood,
                    address == null ? null : address.City,
                    address == null ? null : address.State,
                    address == null ? null : address.Complement);

            return await query.ToListAsync(cancellationToken);
        }
    }
}
