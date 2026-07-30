using ApiCep.Application.Address.Queries.GetAddressById;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using NSubstitute;
using AddressEntity = ApiCep.Domain.Entities.Address;

namespace ApiCep.Tests.Application.Address.Queries.GetAddressById
{
    public sealed class GetAddressByIdQueryHandlerTests
    {
        private readonly IAddressRepository _addressRepository;
        private readonly GetAddressByIdQueryHandler _handler;

        public GetAddressByIdQueryHandlerTests()
        {
            _addressRepository = Substitute.For<IAddressRepository>();
            _handler = new GetAddressByIdQueryHandler(_addressRepository);
        }

        [Fact]
        public async Task Handle_ShouldReturnAddress_WhenAddressBelongsToUser()
        {
            var userId = Guid.NewGuid();
            var address = new AddressEntity(userId, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", "Apartamento 10");
            var query = new GetAddressByIdQuery(userId, address.Id);

            _addressRepository.GetByIdAsync(query.AddressId, Arg.Any<CancellationToken>()).Returns(address);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(address.Id, response.Id);
            Assert.Equal(userId, response.UserId);
            Assert.Equal("01310100", response.ZipCode);
            Assert.Equal("Avenida Paulista", response.Street);
            Assert.Equal("1000", response.Number);
            Assert.Equal("Bela Vista", response.Neighborhood);
            Assert.Equal("São Paulo", response.City);
            Assert.Equal("SP", response.State);
            Assert.Equal("Apartamento 10", response.Complement);
            Assert.True(response.IsActive);

            await _addressRepository.Received(1).GetByIdAsync(query.AddressId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAddressDoesNotExist()
        {
            var query = new GetAddressByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            _addressRepository.GetByIdAsync(query.AddressId, Arg.Any<CancellationToken>()).Returns((AddressEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Endereço não encontrado.", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAddressBelongsToAnotherUser()
        {
            var addressOwnerId = Guid.NewGuid();
            var requestingUserId = Guid.NewGuid();
            var address = new AddressEntity(addressOwnerId, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", null);
            var query = new GetAddressByIdQuery(requestingUserId, address.Id);

            _addressRepository.GetByIdAsync(query.AddressId, Arg.Any<CancellationToken>()).Returns(address);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("Endereço não encontrado.", exception.Message);
        }
    }
}
