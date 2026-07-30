using FluentValidation;


namespace ApiCep.Application.Address.Queries.ListAddressesByUser
{
    public sealed class ListAddressesByUserQueryValidator : AbstractValidator<ListAddressesByUserQuery>
    {
        public ListAddressesByUserQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatório.");
        }
    }
}
