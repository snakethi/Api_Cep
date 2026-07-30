using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;
namespace ApiCep.Application.Address.Queries.ListAddressesByUser
{
    public sealed class ListAddressesByUserQueryHandler : IRequestHandler<ListAddressesByUserQuery, IReadOnlyCollection<AddressResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;

        public ListAddressesByUserQueryHandler(IUserRepository userRepository, IAddressRepository addressRepository)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
        }

        public async Task<IReadOnlyCollection<AddressResponse>> Handle(ListAddressesByUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                throw new NotFoundException("Usuário não encontrado.");

            var addresses = await _addressRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            return addresses.Select(address => new AddressResponse(address.Id, address.UserId, address.ZipCode, address.Street, address.Number, address.Neighborhood, address.City, address.State, address.Complement, address.IsActive, address.CreatedAtUtc, address.UpdatedAtUtc)).ToArray();
        }
    }
}
