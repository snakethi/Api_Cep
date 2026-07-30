using ApiCep.Application.Address.Models;
using MediatR;

namespace ApiCep.Application.Address.Queries.GetAddressById
{
    public sealed record GetAddressByIdQuery(Guid UserId, Guid AddressId) : IRequest<AddressResponse>;
}
