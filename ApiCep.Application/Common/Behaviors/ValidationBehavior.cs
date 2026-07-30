using MediatR;
using FluentValidation;

namespace ApiCep.Application.Common.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>where TRequest : notnull
    {
        private readonly IReadOnlyCollection<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators.ToArray();
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Count == 0)
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
                .ToList();

            if (failures.Count > 0)
                throw new ValidationException(failures);

            return await next();
        }
    }
}
