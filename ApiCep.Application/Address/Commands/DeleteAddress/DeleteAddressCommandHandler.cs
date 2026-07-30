using ApiCep.Application.Common.Exceptions;
using ApiCep.Application.Interfaces.Repositories;
using MediatR;

namespace ApiCep.Application.Address.Commands.DeleteAddress
{
    public sealed class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand>
    {
        private readonly IAddressRepository _addressRepository;

        public DeleteAddressCommandHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId, cancellationToken);

            if (address is null || address.UserId != request.UserId)
                throw new NotFoundException("Endereço não encontrado.");

            address.Deactivate();

            await _addressRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
