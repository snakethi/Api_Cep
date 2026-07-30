using ApiCep.Application.Address.Queries.ListAddressesByUser;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using NSubstitute;
using AddressEntity = ApiCep.Domain.Entities.Address;
using UserEntity = ApiCep.Domain.Entities.User;

namespace ApiCep.Tests.Application.Address.Queries.ListAddressesByUser
{
    public sealed class ListAddressesByUserQueryHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ListAddressesByUserQueryHandler _handler;

        public ListAddressesByUserQueryHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _addressRepository = Substitute.For<IAddressRepository>();
            _handler = new ListAddressesByUserQueryHandler(_userRepository, _addressRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnUserAddresses_WhenUserExists()
        {
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var firstAddress = new AddressEntity(user.Id, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", "Apartamento 10");
            var secondAddress = new AddressEntity(user.Id, "04538-133", "Avenida Brigadeiro Faria Lima", "200", "Itaim Bibi", "São Paulo", "SP", null);
            var query = new ListAddressesByUserQuery(user.Id);

            _userRepository.GetByIdAsync(query.UserId, Arg.Any<CancellationToken>()).Returns(user);
            _addressRepository.GetByUserIdAsync(query.UserId, Arg.Any<CancellationToken>()).Returns(new[] { firstAddress, secondAddress });

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, response.Count);

            var firstResponse = response.First();

            Assert.Equal(firstAddress.Id, firstResponse.Id);
            Assert.Equal(user.Id, firstResponse.UserId);
            Assert.Equal("01310100", firstResponse.ZipCode);
            Assert.Equal("Avenida Paulista", firstResponse.Street);
            Assert.Equal("1000", firstResponse.Number);
            Assert.Equal("Bela Vista", firstResponse.Neighborhood);
            Assert.Equal("São Paulo", firstResponse.City);
            Assert.Equal("SP", firstResponse.State);
            Assert.Equal("Apartamento 10", firstResponse.Complement);
            Assert.True(firstResponse.IsActive);

            await _userRepository.Received(1).GetByIdAsync(query.UserId, Arg.Any<CancellationToken>());
            await _addressRepository.Received(1).GetByUserIdAsync(query.UserId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyCollection_WhenUserHasNoAddresses()
        {
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var query = new ListAddressesByUserQuery(user.Id);

            _userRepository.GetByIdAsync(query.UserId, Arg.Any<CancellationToken>()).Returns(user);
            _addressRepository.GetByUserIdAsync(query.UserId, Arg.Any<CancellationToken>()).Returns(Array.Empty<AddressEntity>());

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Empty(response);

            await _addressRepository.Received(1).GetByUserIdAsync(query.UserId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var query = new ListAddressesByUserQuery(Guid.NewGuid());

            _userRepository.GetByIdAsync(query.UserId, Arg.Any<CancellationToken>()).Returns((UserEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Usuário não encontrado.", exception.Message);

            await _addressRepository.DidNotReceive().GetByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }
    }
}
