using FluentValidation;

namespace ApiCep.Application.User.Commands.DeleteUser
{
    public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatório.");
        }
    }
}
