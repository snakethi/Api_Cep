using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Commands.UpdateUser;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.User.Commands.UpdateUser
{
    public sealed class UpdateUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new UpdateUserCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_ShouldUpdateUser_WhenUserExistsAndEmailIsAvailable()
        {
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var command = new UpdateUserCommand(user.Id, "Thiago Atualizado", "THIAGO.NOVO@TESTE.COM");

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
            _userRepository.EmailExistsAsync(command.Email, command.Id, Arg.Any<CancellationToken>()).Returns(false);

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(user.Id, response.Id);
            Assert.Equal("Thiago Atualizado", response.Name);
            Assert.Equal("thiago.novo@teste.com", response.Email);
            Assert.NotNull(response.UpdatedAtUtc);

            await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var command = new UpdateUserCommand(Guid.NewGuid(), "Thiago Atualizado", "thiago.novo@teste.com");

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((UserEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Usuário não encontrado.", exception.Message);

            await _userRepository.DidNotReceive().EmailExistsAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>());
            await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var command = new UpdateUserCommand(user.Id, "Thiago Atualizado", "email.existente@teste.com");

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
            _userRepository.EmailExistsAsync(command.Email, command.Id, Arg.Any<CancellationToken>()).Returns(true);

            var exception = await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Já existe um usuário cadastrado com este e-mail.", exception.Message);
            Assert.Equal("Thiago Botaro", user.Name);
            Assert.Equal("thiago@teste.com", user.Email);

            await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
