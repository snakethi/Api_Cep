namespace ApiCep.Application.Address.Models
{
    public sealed record ViaCepAddressResult(string ZipCode, string Street, string Neighborhood, string City, string State);
}
