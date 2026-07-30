using ApiCep.Application.Address.Models;
using ApiCep.Application.Address.Queries.GetAddressByZipCode;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.ExternalServices;
using NSubstitute;


namespace ApiCep.Tests.Application.Address.Queries.GetAddressByZipCode
{
    public sealed class GetAddressByZipCodeQueryHandlerTests
    {
        private readonly IViaCepService _viaCepService;
        private readonly GetAddressByZipCodeQueryHandler _handler;

        public GetAddressByZipCodeQueryHandlerTests()
        {
            _viaCepService = Substitute.For<IViaCepService>();
            _handler = new GetAddressByZipCodeQueryHandler(_viaCepService);
        }

        [Fact]
        public async Task Handle_ShouldReturnAddress_WhenZipCodeExists()
        {
            var query = new GetAddressByZipCodeQuery("01310-100");
            var viaCepResult = new ViaCepAddressResult("01310100", "Avenida Paulista", "Bela Vista", "São Paulo", "SP");

            _viaCepService.GetAddressAsync(query.ZipCode, Arg.Any<CancellationToken>()).Returns(viaCepResult);

            var response = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("01310100", response.ZipCode);
            Assert.Equal("Avenida Paulista", response.Street);
            Assert.Equal("Bela Vista", response.Neighborhood);
            Assert.Equal("São Paulo", response.City);
            Assert.Equal("SP", response.State);

            await _viaCepService.Received(1).GetAddressAsync(query.ZipCode, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenZipCodeDoesNotExist()
        {
            var query = new GetAddressByZipCodeQuery("00000-000");

            _viaCepService.GetAddressAsync(query.ZipCode, Arg.Any<CancellationToken>()).Returns((ViaCepAddressResult?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));

            Assert.Equal("CEP não encontrado.", exception.Message);
        }
    }
}
