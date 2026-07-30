namespace ApiCep.Application.Address.Models
{
    public sealed record UpdateAddressRequest(string ZipCode, string Number, string? Complement = null, string? Street = null, string? Neighborhood = null);
}
