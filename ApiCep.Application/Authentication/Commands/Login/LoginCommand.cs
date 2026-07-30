using ApiCep.Application.Authentication.Models;
using MediatR;

namespace ApiCep.Application.Authentication.Commands.Login
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
}
