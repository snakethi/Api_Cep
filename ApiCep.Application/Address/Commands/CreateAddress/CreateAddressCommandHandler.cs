using ApiCep.Application.Address.Models;
using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.ExternalServices;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;
using AddressEntity = ApiCep.Domain.Entities.Address;


namespace ApiCep.Application.Address.Commands.CreateAddress
{
    public sealed class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IViaCepService _viaCepService;

        public CreateAddressCommandHandler(IUserRepository userRepository, IAddressRepository addressRepository, IViaCepService viaCepService)
        {
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _viaCepService = viaCepService;
        }

        public async Task<AddressResponse> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                throw new NotFoundException("Usuário não encontrado.");

            var viaCepAddress = await _viaCepService.GetAddressAsync(request.ZipCode, cancellationToken);

            if (viaCepAddress is null)
                throw new NotFoundException("CEP não encontrado.");

            var street = string.IsNullOrWhiteSpace(viaCepAddress.Street) ? request.Street : viaCepAddress.Street;
            var neighborhood = string.IsNullOrWhiteSpace(viaCepAddress.Neighborhood) ? request.Neighborhood : viaCepAddress.Neighborhood;

            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("O logradouro deve ser informado para este CEP.");

            if (string.IsNullOrWhiteSpace(neighborhood))
                throw new ArgumentException("O bairro deve ser informado para este CEP.");

            var address = new AddressEntity(request.UserId, viaCepAddress.ZipCode, street, request.Number, neighborhood, viaCepAddress.City, viaCepAddress.State, request.Complement);

            await _addressRepository.AddAsync(address, cancellationToken);
            await _addressRepository.SaveChangesAsync(cancellationToken);

            return new AddressResponse(address.Id, address.UserId, address.ZipCode, address.Street, address.Number, address.Neighborhood, address.City, address.State, address.Complement, address.IsActive, address.CreatedAtUtc, address.UpdatedAtUtc);
        }
    }
}
