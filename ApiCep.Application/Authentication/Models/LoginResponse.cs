using ApiCep.Application.User.Models;

namespace ApiCep.Application.Authentication.Models
{
    public sealed record LoginResponse(string AccessToken,string TokenType,DateTime ExpiresAtUtc, UserResponse User);
}
