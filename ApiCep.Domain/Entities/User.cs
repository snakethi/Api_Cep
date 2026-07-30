

namespace ApiCep.Domain.Entities
{
    public sealed class User
    {
        private User()
        {
        }

        public User(string name, string email,string passwordHash)
        {
            Id = Guid.NewGuid();

            SetName(name);
            SetEmail(email);
            SetPasswordHash(passwordHash);

            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; private set; } = string.Empty;

        public bool IsActive { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime? UpdatedAtUtc { get; private set; }

        public DateTime? DeletedAtUtc { get; private set; }

        public void Update(string name, string email)
        {
            SetName(name);
            SetEmail(email);

            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void ChangePasswordHash(string passwordHash)
        {
            SetPasswordHash(passwordHash);

            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            DeletedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do usuário é obrigatório.",nameof(name));

            Name = name.Trim();
        }

        private void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException( "O e-mail do usuário é obrigatório.",nameof(email));

            Email = email.Trim().ToLowerInvariant();
        }

        private void SetPasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("O hash da senha é obrigatório.",nameof(passwordHash));

            PasswordHash = passwordHash;
        }
    }
}
