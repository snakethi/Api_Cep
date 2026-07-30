using ApiCep.Application.Address.Commands.DeleteAddress;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using NSubstitute;
using AddressEntity = ApiCep.Domain.Entities.Address;
namespace ApiCep.Tests.Application.Address.Commands.DeleteAddress
{
    public sealed class DeleteAddressCommandHandlerTests
    {
        private readonly IAddressRepository _addressRepository;
        private readonly DeleteAddressCommandHandler _handler;

        public DeleteAddressCommandHandlerTests()
        {
            _addressRepository = Substitute.For<IAddressRepository>();
            _handler = new DeleteAddressCommandHandler(_addressRepository);
        }

        [Fact]
        public async Task Handle_ShouldDeactivateAddress_WhenAddressBelongsToUser()
        {
            var userId = Guid.NewGuid();
            var address = new AddressEntity(userId, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", "Apartamento 10");
            var command = new DeleteAddressCommand(userId, address.Id);

            _addressRepository.GetByIdAsync(command.AddressId, Arg.Any<CancellationToken>()).Returns(address);

            await _handler.Handle(command, CancellationToken.None);

            Assert.False(address.IsActive);
            Assert.NotNull(address.DeletedAtUtc);
            Assert.NotNull(address.UpdatedAtUtc);

            await _addressRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAddressDoesNotExist()
        {
            var command = new DeleteAddressCommand(Guid.NewGuid(), Guid.NewGuid());

            _addressRepository.GetByIdAsync(command.AddressId, Arg.Any<CancellationToken>()).Returns((AddressEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Endereço não encontrado.", exception.Message);

            await _addressRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAddressBelongsToAnotherUser()
        {
            var addressOwnerId = Guid.NewGuid();
            var requestingUserId = Guid.NewGuid();
            var address = new AddressEntity(addressOwnerId, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", null);
            var command = new DeleteAddressCommand(requestingUserId, address.Id);

            _addressRepository.GetByIdAsync(command.AddressId, Arg.Any<CancellationToken>()).Returns(address);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Endereço não encontrado.", exception.Message);
            Assert.True(address.IsActive);
            Assert.Null(address.DeletedAtUtc);

            await _addressRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
