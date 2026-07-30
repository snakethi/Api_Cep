using FluentValidation;

namespace ApiCep.Application.User.Commands.CreateUser
{
    public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("A senha é obrigatória.")
                .MinimumLength(8)
                .WithMessage("A senha deve possuir no mínimo 8 caracteres.")
                .MaximumLength(100)
                .WithMessage("A senha deve possuir no máximo 100 caracteres.")
                .Matches("[A-Z]")
                .WithMessage("A senha deve possuir ao menos uma letra maiúscula.")
                .Matches("[a-z]")
                .WithMessage("A senha deve possuir ao menos uma letra minúscula.")
                .Matches("[0-9]")
                .WithMessage("A senha deve possuir ao menos um número.")
                .Matches(@"[\W_]")
                .WithMessage("A senha deve possuir ao menos um caractere especial.");
        }
    }
}
