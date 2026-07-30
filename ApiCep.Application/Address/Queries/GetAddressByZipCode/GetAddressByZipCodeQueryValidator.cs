using FluentValidation;


namespace ApiCep.Application.Address.Queries.GetAddressByZipCode
{
    public sealed class GetAddressByZipCodeQueryValidator : AbstractValidator<GetAddressByZipCodeQuery>
    {
        public GetAddressByZipCodeQueryValidator()
        {
            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage("O CEP é obrigatório.")
                .Must(HaveEightDigits)
                .WithMessage("O CEP deve possuir 8 dígitos.");
        }

        private static bool HaveEightDigits(string zipCode)
        {
            return new string(zipCode.Where(char.IsDigit).ToArray()).Length == 8;
        }
    }
}
