using ApiCep.Application.Address.Commands.UpdateAddress;
using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.ExternalServices;
using ApiCep.Application.Interfaces.Repositories;
using NSubstitute;
using AddressEntity = ApiCep.Domain.Entities.Address;

namespace ApiCep.Tests.Application.Address.Commands.UpdateAddress
{
    public sealed class UpdateAddressCommandHandlerTests
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IViaCepService _viaCepService;
        private readonly UpdateAddressCommandHandler _handler;

        public UpdateAddressCommandHandlerTests()
        {
            _addressRepository = Substitute.For<IAddressRepository>();
            _viaCepService = Substitute.For<IViaCepService>();
            _handler = new UpdateAddressCommandHandler(_addressRepository, _viaCepService);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAddress_WhenAddressAndZipCodeExist()
        {
            var userId = Guid.NewGuid();
            var address = new AddressEntity(userId, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", "Apartamento 10");
            var command = new UpdateAddressCommand(userId, address.Id, "04538-133", "200", "Conjunto 20", null, null);
            var viaCepResult = new ViaCepAddressResult("04538133", "Avenida Brigadeiro Faria Lima", "Itaim Bibi", "São Paulo", "SP");

            _addressRepository.GetByIdAsync(command.AddressId, Arg.Any<CancellationToken>()).Returns(address);
            _viaCepService.GetAddressAsync(command.ZipCode, Arg.Any<CancellationToken>()).Returns(viaCepResult);

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(address.Id, response.Id);
            Assert.Equal(userId, response.UserId);
            Assert.Equal("04538133", response.ZipCode);
            Assert.Equal("Avenida Brigadeiro Faria Lima", response.Street);
            Assert.Equal("200", response.Number);
            Assert.Equal("Itaim Bibi", response.Neighborhood);
            Assert.Equal("São Paulo", response.City);
            Assert.Equal("SP", response.State);
            Assert.Equal("Conjunto 20", response.Complement);
            Assert.NotNull(response.UpdatedAtUtc);

            await _addressRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenAddressDoesNotExist()
        {
            var command = new UpdateAddressCommand(Guid.NewGuid(), Guid.NewGuid(), "01310-100", "1000", null, null, null);

            _addressRepository.GetByIdAsync(command.AddressId, Arg.Any<CancellationToken>()).Returns((AddressEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Endereço não encontrado.", exception.Message);

            await _viaCepService.DidNotReceive().GetAddressAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _addressRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenZipCodeDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var address = new AddressEntity(userId, "01310-100", "Avenida Paulista", "1000", "Bela Vista", "São Paulo", "SP", null);
            var command = new UpdateAddressCommand(userId, address.Id, "00000-000", "200", null, null, null);

            _addressRepository.GetByIdAsync(command.AddressId, Arg.Any<CancellationToken>()).Returns(address);
            _viaCepService.GetAddressAsync(command.ZipCode, Arg.Any<CancellationToken>()).Returns((ViaCepAddressResult?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("CEP não encontrado.", exception.Message);
            Assert.Equal("01310100", address.ZipCode);
            Assert.Equal("Avenida Paulista", address.Street);

            await _addressRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
