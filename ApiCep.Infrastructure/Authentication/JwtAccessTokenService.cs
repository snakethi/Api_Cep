
using ApiCep.Application.Authentication.Models;
using ApiCep.Application.Interfaces.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiCep.Infrastructure.Authentication
{
    public sealed class JwtAccessTokenService : IAccessTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtAccessTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public AccessTokenResult Generate(Guid userId, string name, string email)
        {
            var issuedAtUtc = DateTime.UtcNow;
            var expiresAtUtc = issuedAtUtc.AddMinutes(_jwtSettings.ExpireMinutes);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name,name),
            new Claim(JwtRegisteredClaimNames.Email,email),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier,userId.ToString())
        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: issuedAtUtc,
                expires: expiresAtUtc,
                signingCredentials: signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AccessTokenResult(accessToken, expiresAtUtc);
        }
    }
}
