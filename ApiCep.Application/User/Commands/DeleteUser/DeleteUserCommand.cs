using MediatR;

namespace ApiCep.Application.User.Commands.DeleteUser
{
    public sealed record DeleteUserCommand(Guid Id) : IRequest;
}
