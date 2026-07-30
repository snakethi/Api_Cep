using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.ExternalServices;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;

namespace ApiCep.Application.Address.Commands.UpdateAddress
{
    public sealed class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressResponse>
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IViaCepService _viaCepService;

        public UpdateAddressCommandHandler(IAddressRepository addressRepository, IViaCepService viaCepService)
        {
            _addressRepository = addressRepository;
            _viaCepService = viaCepService;
        }

        public async Task<AddressResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);

            if (address is null || address.UserId != request.UserId)
                throw new NotFoundException("Endereço não encontrado.");

            var viaCepAddress = await _viaCepService.GetAddressAsync(request.ZipCode, cancellationToken);

            if (viaCepAddress is null)
                throw new NotFoundException("CEP não encontrado.");

            var street = string.IsNullOrWhiteSpace(viaCepAddress.Street) ? request.Street : viaCepAddress.Street;
            var neighborhood = string.IsNullOrWhiteSpace(viaCepAddress.Neighborhood) ? request.Neighborhood : viaCepAddress.Neighborhood;

            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("O logradouro deve ser informado para este CEP.");

            if (string.IsNullOrWhiteSpace(neighborhood))
                throw new ArgumentException("O bairro deve ser informado para este CEP.");

            address.Update(viaCepAddress.ZipCode, street, request.Number, neighborhood, viaCepAddress.City, viaCepAddress.State, request.Complement);

            await _addressRepository.SaveChangesAsync(cancellationToken);

            return new AddressResponse(address.Id, address.UserId, address.ZipCode, address.Street, address.Number, address.Neighborhood, address.City, address.State, address.Complement, address.IsActive, address.CreatedAtUtc, address.UpdatedAtUtc);
        }
    }
}
