using FluentValidation;

namespace ApiCep.Application.Authentication.Commands.Login
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório.")
                .EmailAddress()
                .WithMessage("O e-mail informado é inválido.")
                .MaximumLength(200)
                .WithMessage("O e-mail deve possuir no máximo 200 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("A senha é obrigatória.")
                .MaximumLength(100)
                .WithMessage("A senha deve possuir no máximo 100 caracteres.");
        }
    }
}
