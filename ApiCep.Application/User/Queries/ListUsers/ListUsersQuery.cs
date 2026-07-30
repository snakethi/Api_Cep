using ApiCep.Application.Common.Models;
using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Queries.ListUsers
{
    public sealed record ListUsersQuery(int Page = 1,int PageSize = 10,string? Search = null,string SortBy = "name",string SortDirection = "asc") : IRequest<PagedResult<UserResponse>>;
}
