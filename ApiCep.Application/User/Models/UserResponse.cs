namespace ApiCep.Application.User.Models
{
    public sealed record UserResponse(Guid Id,string Name,string Email,bool IsActive,DateTime CreatedAtUtc,DateTime? UpdatedAtUtc);
}
