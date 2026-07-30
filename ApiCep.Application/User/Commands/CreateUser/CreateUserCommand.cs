using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Commands.CreateUser
{
    public sealed record CreateUserCommand(string Name, string Email, string Password) : IRequest<UserResponse>;
}
