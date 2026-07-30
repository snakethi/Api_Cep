using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using ApiCep.Application.User.Queries.GetUserById;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.User.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnUser_WhenUserExists()
        {
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var query = new GetUserByIdQuery(user.Id);

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(user);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(user.Id, response.Id);
            Assert.Equal(user.Name, response.Name);
            Assert.Equal(user.Email, response.Email);
            Assert.Equal(user.IsActive, response.IsActive);
            Assert.Equal(user.CreatedAtUtc, response.CreatedAtUtc);
            Assert.Equal(user.UpdatedAtUtc, response.UpdatedAtUtc);

            await _userRepository.Received(1).GetByIdAsync(query.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());

            _userRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((UserEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Usuário não encontrado.", exception.Message);
        }
    }
}
