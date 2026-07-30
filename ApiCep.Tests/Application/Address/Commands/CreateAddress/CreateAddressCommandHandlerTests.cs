using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.ExternalServices;
using ApiCep.Application.Interfaces.Repositories;
using NSubstitute;
using UserEntity = ApiCep.Domain.Entities.User;
using AddressEntity = ApiCep.Domain.Entities.Address;
using ApiCep.Application.Address.Commands.CreateAddress;

namespace ApiCep.Tests.Application.Address.Commands.CreateAddress
{
    public sealed class CreateAddressCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IViaCepService _viaCepService;
        private readonly CreateAddressCommandHandler _handler;

        public CreateAddressCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _addressRepository = Substitute.For<IAddressRepository>();
            _viaCepService = Substitute.For<IViaCepService>();
            _handler = new CreateAddressCommandHandler(_userRepository, _addressRepository, _viaCepService);
        }

        [Fact]
        public async Task Handle_ShouldCreateAddress_WhenUserAndZipCodeExist()
        {
            var userId = Guid.NewGuid();
            var command = new CreateAddressCommand(userId, "01310-100", "1000", "Apartamento 10", null, null);
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");
            var viaCepResult = new ViaCepAddressResult("01310100", "Avenida Paulista", "Bela Vista", "São Paulo", "SP");

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _viaCepService.GetAddressAsync(command.ZipCode, Arg.Any<CancellationToken>()).Returns(viaCepResult);

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal(userId, response.UserId);
            Assert.Equal("01310100", response.ZipCode);
            Assert.Equal("Avenida Paulista", response.Street);
            Assert.Equal("1000", response.Number);
            Assert.Equal("Bela Vista", response.Neighborhood);
            Assert.Equal("São Paulo", response.City);
            Assert.Equal("SP", response.State);
            Assert.Equal("Apartamento 10", response.Complement);
            Assert.True(response.IsActive);

            await _addressRepository.Received(1).AddAsync(Arg.Is<AddressEntity>(address => address != null && address.UserId == userId && address.ZipCode == "01310100" && address.Street == "Avenida Paulista"), Arg.Any<CancellationToken>());
            await _addressRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var command = new CreateAddressCommand(userId, "01310-100", "1000", null, null, null);

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((UserEntity?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Usuário não encontrado.", exception.Message);

            await _viaCepService.DidNotReceive().GetAddressAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            await _addressRepository.DidNotReceive().AddAsync(Arg.Any<AddressEntity>(), Arg.Any<CancellationToken>());
            await _addressRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenZipCodeDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var command = new CreateAddressCommand(userId, "00000-000", "1000", null, null, null);
            var user = new UserEntity("Thiago Botaro", "thiago@teste.com", "hash-da-senha");

            _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);
            _viaCepService.GetAddressAsync(command.ZipCode, Arg.Any<CancellationToken>()).Returns((ViaCepAddressResult?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.Equal("CEP não encontrado.", exception.Message);

            await _addressRepository.DidNotReceive().AddAsync(Arg.Any<AddressEntity>(), Arg.Any<CancellationToken>());
            await _addressRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
