using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.Interfaces.Security;
using ApiCep.Application.User.Models;
using MediatR;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Application.User.Commands.CreateUser
{
    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email, null, cancellationToken);

            if (emailExists)
                throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");

            var passwordHash = _passwordHasherService.Hash(request.Password);
            var user = new UserEntity(request.Name, request.Email, passwordHash);

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return new UserResponse(user.Id,user.Name,user.Email,user.IsActive,user.CreatedAtUtc,user.UpdatedAtUtc);
        }
    }

}
