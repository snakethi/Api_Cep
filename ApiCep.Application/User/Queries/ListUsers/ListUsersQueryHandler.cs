
using ApiCep.Application.Common.Models;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Queries.ListUsers
{
    public sealed class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PagedResult<UserResponse>>
    {
        private readonly IUserRepository _userRepository;

        public ListUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResult<UserResponse>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetPagedAsync(request.Page, request.PageSize, request.Search, request.SortBy, request.SortDirection, cancellationToken);

            var users = result.Items.Select(user => new UserResponse(user.Id,user.Name,user.Email,user.IsActive,user.CreatedAtUtc, user.UpdatedAtUtc)).ToArray();

            return new PagedResult<UserResponse>(users, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
        }
    }
}
