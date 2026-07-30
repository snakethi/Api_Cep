using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.Interfaces.Security;
using ApiCep.Application.User.Commands.CreateUser;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.User.Commands.CreateUser
{
    public sealed class CreateUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _passwordHasherService = Substitute.For<IPasswordHasherService>();
            _handler = new CreateUserCommandHandler(_userRepository, _passwordHasherService);
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            var command = new CreateUserCommand("Thiago Botaro", "thiago@teste.com", "Teste@123");
            const string passwordHash = "hash-gerado";

            _userRepository.EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>()).Returns(false);
            _passwordHasherService.Hash(command.Password).Returns(passwordHash);

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal("Thiago Botaro", response.Name);
            Assert.Equal("thiago@teste.com", response.Email);
            Assert.True(response.IsActive);
            Assert.Null(response.UpdatedAtUtc);

            _passwordHasherService.Received(1).Hash(command.Password);
            await _userRepository.Received(1).AddAsync(Arg.Is<UserEntity>(user => user != null && user.Name == command.Name && user.Email == command.Email && user.PasswordHash == passwordHash), Arg.Any<CancellationToken>());
            await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var command = new CreateUserCommand("Thiago Botaro", "thiago@teste.com", "Teste@123");

            _userRepository.EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>()).Returns(true);

            var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Já existe um usuário cadastrado com este e-mail.", exception.Message);

            _passwordHasherService.DidNotReceive().Hash(Arg.Any<string>());
            await _userRepository.DidNotReceive().AddAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>());
            await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
