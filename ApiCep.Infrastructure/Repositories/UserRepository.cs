using ApiCep.Application.Common.Models;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Domain.Entities;
using ApiCep.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCep.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Id == id && x.DeletedAtUtc == null, cancellationToken);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == normalizedEmail && x.DeletedAtUtc == null, cancellationToken);
        }

        public Task<bool> EmailExistsAsync(string email, Guid? ignoredUserId = null, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return _context.Users.AsNoTracking().AnyAsync(x => x.Email == normalizedEmail && (!ignoredUserId.HasValue || x.Id != ignoredUserId.Value), cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResult<User>> GetPagedAsync(int page,int pageSize,string? search,string sortBy,string sortDirection,CancellationToken cancellationToken = default)
        {
            var query = _context.Users.AsNoTracking().Where(x => x.DeletedAtUtc == null);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();

                query = query.Where(x =>x.Name.Contains(searchTerm) || x.Email.Contains(searchTerm));
            }

            var normalizedSortBy = sortBy.Trim().ToLowerInvariant();
            var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = (normalizedSortBy, descending) switch
            {
                ("email", true) => query.OrderByDescending(x => x.Email),
                ("email", false) => query.OrderBy(x => x.Email),
                ("createdatutc", true) => query.OrderByDescending(x => x.CreatedAtUtc),
                ("createdatutc", false) => query.OrderBy(x => x.CreatedAtUtc),
                ("name", true) => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Name)
            };

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new PagedResult<User>(items, page,pageSize,totalItems,totalPages);
        }

    }
}
