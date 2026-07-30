using ApiCep.Application.Address.Models;
using MediatR;

namespace ApiCep.Application.Address.Queries.ListAddressesByUser
{
    public sealed record ListAddressesByUserQuery(Guid UserId) : IRequest<IReadOnlyCollection<AddressResponse>>;
}
