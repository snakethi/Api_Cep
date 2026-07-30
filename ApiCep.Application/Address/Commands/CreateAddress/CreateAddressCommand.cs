using ApiCep.Application.Address.Models;
using MediatR;

namespace ApiCep.Application.Address.Commands.CreateAddress
{
    public sealed record CreateAddressCommand(Guid UserId, string ZipCode, string Number, string? Complement, string? Street, string? Neighborhood) : IRequest<AddressResponse>;
}
