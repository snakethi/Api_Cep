using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;

namespace ApiCep.Application.Address.Queries.GetAddressById
{
    public sealed class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, AddressResponse>
    {
        private readonly IAddressRepository _addressRepository;

        public GetAddressByIdQueryHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<AddressResponse> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);

            if (address is null || address.UserId != request.UserId)
                throw new NotFoundException("Endereço não encontrado.");

            return new AddressResponse(address.Id, address.UserId, address.ZipCode, address.Street, address.Number, address.Neighborhood, address.City, address.State, address.Complement, address.IsActive, address.CreatedAtUtc, address.UpdatedAtUtc);
        }
    }
}
