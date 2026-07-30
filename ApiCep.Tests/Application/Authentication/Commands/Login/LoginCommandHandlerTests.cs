using ApiCep.Application.Authentication.Commands.Login;
using ApiCep.Application.Authentication.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.Interfaces.Security;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.Authentication.Commands.Login
{
    public sealed class LoginCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IAccessTokenService _accessTokenService;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordHasherService = Substitute.For<IPasswordHasherService>();
            _accessTokenService = Substitute.For<IAccessTokenService>();
            _handler = new LoginCommandHandler(_userRepository, _passwordHasherService, _accessTokenService);
        }

        [Fact]
        public async Task Handle_ShouldReturnAccessToken_WhenCredentialsAreValid()
        {
            var command = new LoginCommand("thiago@teste.com", "Teste@123");
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var expiresAtUtc = DateTime.UtcNow.AddHours(2);
            var tokenResult = new AccessTokenResult("token-jwt", expiresAtUtc);

            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasherService.Verify(command.Password, user.PasswordHash).Returns(true);
            _accessTokenService.Generate(user.Id, user.Name, user.Email).Returns(tokenResult);

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal("token-jwt", response.AccessToken);
            Assert.Equal("Bearer", response.TokenType);
            Assert.Equal(expiresAtUtc, response.ExpiresAtUtc);
            Assert.Equal(user.Id, response.User.Id);
            Assert.Equal(user.Name, response.User.Name);
            Assert.Equal(user.Email, response.User.Email);

            _passwordHasherService.Received(1).Verify(command.Password, user.PasswordHash);
            _accessTokenService.Received(1).Generate(user.Id, user.Name, user.Email);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
        {
            var command = new LoginCommand("thiago@teste.com", "SenhaErrada");
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");

            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasherService.Verify(command.Password, user.PasswordHash).Returns(false);

            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("E-mail ou senha inválidos.", exception.Message);

            _accessTokenService.DidNotReceive().Generate(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserDoesNotExist()
        {
            var command = new LoginCommand("inexistente@teste.com", "Teste@123");

            _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((UserEntity?)null);

            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("E-mail ou senha inválidos.", exception.Message);

            _passwordHasherService.DidNotReceive().Verify(Arg.Any<string>(), Arg.Any<string>());
            _accessTokenService.DidNotReceive().Generate(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
