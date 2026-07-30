namespace ApiCep.Application.Authentication.Models
{
    public sealed record AccessTokenResult(string AccessToken, DateTime ExpiresAtUtc);
}
