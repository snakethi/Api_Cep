using ApiCep.Application.Authentication.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.Interfaces.Security;
using ApiCep.Application.User.Models;
using MediatR;
namespace ApiCep.Application.Authentication.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IAccessTokenService _accessTokenService;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasherService passwordHasherService, IAccessTokenService accessTokenService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _accessTokenService = accessTokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null || !_passwordHasherService.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("E-mail ou senha inválidos.");

            var accessToken = _accessTokenService.Generate(user.Id, user.Name, user.Email);

            var userResponse = new UserResponse(user.Id,user.Name,user.Email,user.IsActive,user.CreatedAtUtc,user.UpdatedAtUtc);

            return new LoginResponse(accessToken.AccessToken,"Bearer",accessToken.ExpiresAtUtc,userResponse);
        }
    }
}
