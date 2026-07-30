using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Commands.UpdateUser
{
    public sealed record UpdateUserCommand(Guid Id, string Name, string Email) : IRequest<UserResponse>;
}
