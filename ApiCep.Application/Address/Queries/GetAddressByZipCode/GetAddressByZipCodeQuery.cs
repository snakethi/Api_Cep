using ApiCep.Application.Address.Models;
using MediatR;

namespace ApiCep.Application.Address.Queries.GetAddressByZipCode
{
    public sealed record GetAddressByZipCodeQuery(string ZipCode) : IRequest<ViaCepAddressResult>;
}
