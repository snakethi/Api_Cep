using ApiCep.Application.Authentication.Models;

namespace ApiCep.Application.Interfaces.Security
{
    public interface IAccessTokenService
    {
        AccessTokenResult Generate(Guid userId, string name, string email);
    }
}
