using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Models;
using MediatR;

namespace ApiCep.Application.User.Commands.UpdateUser
{
    public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                throw new NotFoundException("Usuário não encontrado.");

            var emailExists = await _userRepository.EmailExistsAsync(request.Email, request.Id, cancellationToken);

            if (emailExists)
                throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");

            user.Update(request.Name, request.Email);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return new UserResponse(user.Id,user.Name,user.Email,user.IsActive,user.CreatedAtUtc,user.UpdatedAtUtc);
        }
    }
}
