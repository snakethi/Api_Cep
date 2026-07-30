namespace ApiCep.Domain.Entities
{
    public sealed class Address
    {
        private Address()
        {
        }

        public Address(Guid userId,string zipCode,string street,string number,string neighborhood,
            string city,string state, string? complement = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException( "O usuário do endereço é obrigatório.",nameof(userId));

            Id = Guid.NewGuid();
            UserId = userId;

            SetAddressData( zipCode,street,number, neighborhood,city, state,complement);

            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string ZipCode { get; private set; } = string.Empty;

        public string Street { get; private set; } = string.Empty;

        public string Number { get; private set; } = string.Empty;

        public string Neighborhood { get; private set; } = string.Empty;

        public string City { get; private set; } = string.Empty;

        public string State { get; private set; } = string.Empty;

        public string? Complement { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime? UpdatedAtUtc { get; private set; }

        public DateTime? DeletedAtUtc { get; private set; }

        public User User { get; private set; } = null!;

        public void Update(string zipCode,string street, string number,string neighborhood,string city, string state,string? complement = null)
        {
            SetAddressData(zipCode, street,number,neighborhood, city,state,complement);

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

        private void SetAddressData(string zipCode,string street,string number,string neighborhood, string city,string state,string? complement)
        {
            ZipCode = NormalizeZipCode(zipCode);
            Street = ValidateRequired(street, "A rua é obrigatória.");
            Number = ValidateRequired(number, "O número é obrigatório.");
            Neighborhood = ValidateRequired( neighborhood,"O bairro é obrigatório.");
            City = ValidateRequired(city, "A cidade é obrigatória.");
            State = NormalizeState(state);
            Complement = string.IsNullOrWhiteSpace(complement)? null : complement.Trim();
        }

        private static string NormalizeZipCode(string zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("O CEP é obrigatório.");

            var normalizedZipCode = new string(
                zipCode.Where(char.IsDigit).ToArray());

            if (normalizedZipCode.Length != 8)
                throw new ArgumentException(
                    "O CEP deve possuir oito números.");

            return normalizedZipCode;
        }

        private static string NormalizeState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("O estado é obrigatório.");

            var normalizedState = state.Trim().ToUpperInvariant();

            if (normalizedState.Length != 2)
                throw new ArgumentException("O estado deve possuir duas letras.");

            return normalizedState;
        }

        private static string ValidateRequired(
            string value,
            string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(errorMessage);

            return value.Trim();
        }
    }
}
