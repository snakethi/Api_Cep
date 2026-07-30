using FluentValidation;


namespace ApiCep.Application.User.Queries.ListUsers
{
    public sealed class ListUsersQueryValidator : AbstractValidator<ListUsersQuery>
    {
        private static readonly string[] AllowedSortFields = ["name", "email", "createdAtUtc"];
        private static readonly string[] AllowedSortDirections = ["asc", "desc"];

        public ListUsersQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("A quantidade de itens por página deve estar entre 1 e 100.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => AllowedSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                .WithMessage("O campo de ordenação deve ser name, email ou createdAtUtc.");

            RuleFor(x => x.SortDirection)
                .Must(direction => AllowedSortDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
                .WithMessage("A direção da ordenação deve ser asc ou desc.");

            RuleFor(x => x.Search)
                .MaximumLength(200)
                .WithMessage("O filtro de pesquisa deve possuir no máximo 200 caracteres.");
        }
    }
}
