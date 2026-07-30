using ApiCep.Application.Address.Models;
using MediatR;


namespace ApiCep.Application.Address.Commands.UpdateAddress
{
    public sealed record UpdateAddressCommand(Guid UserId, Guid AddressId, string ZipCode, string Number, string? Complement, string? Street, 
                                              string? Neighborhood) : IRequest<AddressResponse>;
}
