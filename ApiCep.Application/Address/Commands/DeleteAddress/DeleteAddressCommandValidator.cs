using FluentValidation;

namespace ApiCep.Application.Address.Commands.DeleteAddress
{
    public sealed class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
    {
        public DeleteAddressCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("O identificador do usuário é obrigatório.");

            RuleFor(x => x.AddressId).NotEmpty().WithMessage("O identificador do endereço é obrigatório.");
        }
    }
}
