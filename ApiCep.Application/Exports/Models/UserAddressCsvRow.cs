namespace ApiCep.Application.Exports.Models
{
    public sealed record UserAddressCsvRow(Guid UserId, string Name, string Email, bool IsActive, Guid? AddressId, string? ZipCode, string? Street, 
                                           string? Number, string? Neighborhood, string? City, string? State, string? Complement);
}
