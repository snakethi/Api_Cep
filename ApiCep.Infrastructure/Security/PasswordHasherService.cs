using ApiCep.Application.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace ApiCep.Infrastructure.Security
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<PasswordHasherService> _passwordHasher = new();

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A senha é obrigatória.", nameof(password));

            return _passwordHasher.HashPassword(this, password);
        }

        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
                return false;

            var result = _passwordHasher.VerifyHashedPassword(this, passwordHash, password);

            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
