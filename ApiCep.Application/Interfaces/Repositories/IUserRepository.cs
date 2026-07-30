using ApiCep.Application.Common.Models;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<bool> EmailExistsAsync(string email, Guid? ignoredUserId = null, CancellationToken cancellationToken = default);

        Task AddAsync(UserEntity user, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<PagedResult<UserEntity>> GetPagedAsync( int page,int pageSize,string? search,string sortBy, string sortDirection, CancellationToken cancellationToken = default);
    }
}
