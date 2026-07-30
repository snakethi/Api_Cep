using FluentValidation;

namespace ApiCep.Application.Address.Commands.UpdateAddress
{
    public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("O identificador do usuário é obrigatório.");

            RuleFor(x => x.AddressId).NotEmpty().WithMessage("O identificador do endereço é obrigatório.");

            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage("O CEP é obrigatório.")
                .Must(HaveEightDigits)
                .WithMessage("O CEP deve possuir 8 dígitos.");

            RuleFor(x => x.Number)
                .NotEmpty()
                .WithMessage("O número é obrigatório.")
                .MaximumLength(20)
                .WithMessage("O número deve possuir no máximo 20 caracteres.");

            RuleFor(x => x.Complement).MaximumLength(100).WithMessage("O complemento deve possuir no máximo 100 caracteres.");

            RuleFor(x => x.Street).MaximumLength(200).WithMessage("O logradouro deve possuir no máximo 200 caracteres.");

            RuleFor(x => x.Neighborhood).MaximumLength(100).WithMessage("O bairro deve possuir no máximo 100 caracteres.");
        }

        private static bool HaveEightDigits(string zipCode)
        {
            return new string(zipCode.Where(char.IsDigit).ToArray()).Length == 8;
        }
    }
}
