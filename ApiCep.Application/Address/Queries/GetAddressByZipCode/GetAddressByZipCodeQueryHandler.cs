using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.ExternalServices;
using MediatR;

namespace ApiCep.Application.Address.Queries.GetAddressByZipCode
{
    public sealed class GetAddressByZipCodeQueryHandler : IRequestHandler<GetAddressByZipCodeQuery, ViaCepAddressResult>
    {
        private readonly IViaCepService _viaCepService;

        public GetAddressByZipCodeQueryHandler(IViaCepService viaCepService)
        {
            _viaCepService = viaCepService;
        }

        public async Task<ViaCepAddressResult> Handle(GetAddressByZipCodeQuery request, CancellationToken cancellationToken)
        {
            var address = await _viaCepService.GetAddressAsync(request.ZipCode, cancellationToken);

            if (address is null)
                throw new NotFoundException("CEP não encontrado.");

            return address;
        }
    }
}
