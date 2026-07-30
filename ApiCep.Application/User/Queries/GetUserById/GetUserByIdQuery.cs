using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Queries.GetUserById
{
    public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserResponse>;
}
