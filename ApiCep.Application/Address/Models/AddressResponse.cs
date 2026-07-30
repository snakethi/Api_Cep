namespace ApiCep.Application.Address.Models
{
    public sealed record AddressResponse(Guid Id, Guid UserId, string ZipCode, string Street, string Number, string Neighborhood, string City, 
                                         string State, string? Complement, bool IsActive, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
}
