using MediatR;

namespace ApiCep.Application.Address.Commands.DeleteAddress
{
    public sealed record DeleteAddressCommand(Guid UserId, Guid AddressId) : IRequest;
}
