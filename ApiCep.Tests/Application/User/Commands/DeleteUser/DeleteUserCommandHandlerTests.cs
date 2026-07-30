using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Commands.DeleteUser;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.User.Commands.DeleteUser
{
    public sealed class DeleteUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new DeleteUserCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_ShouldDeactivateUser_WhenUserExists()
        {
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var command = new DeleteUserCommand(user.Id);

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);

            await _handler.Handle(command, CancellationToken.None);

            Assert.False(user.IsActive);
            Assert.NotNull(user.DeletedAtUtc);
            Assert.NotNull(user.UpdatedAtUtc);

            await _userRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var command = new DeleteUserCommand(Guid.NewGuid());

            _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((UserEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Usuário não encontrado.", exception.Message);

            await _userRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
