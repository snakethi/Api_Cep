using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;

namespace ApiCep.Application.User.Commands.DeleteUser
{
    public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                throw new NotFoundException("Usuário não encontrado.");

            user.Deactivate();

            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
