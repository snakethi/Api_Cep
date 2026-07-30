using FluentValidation;

namespace ApiCep.Application.User.Commands.UpdateUser
{
    public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome é obrigatório.")
                .MaximumLength(150)
                .WithMessage("O nome deve possuir no máximo 150 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório.")
                .EmailAddress()
                .WithMessage("O e-mail informado é inválido.")
                .MaximumLength(200)
                .WithMessage("O e-mail deve possuir no máximo 200 caracteres.");
        }
    }
}
