using ApiCep.Application.Common.Behaviors;
using FluentValidation;
using MediatR;

namespace ApiCep.Tests.Application.Common.Behaviors
{
    public sealed class ValidationBehaviorTests
    {
        [Fact]
        public async Task Handle_ShouldExecuteNext_WhenThereAreNoValidators()
        {
            var request = new TestRequest("Thiago");
            var expectedResponse = new TestResponse("Executado");
            var behavior = new ValidationBehavior<TestRequest, TestResponse>(Array.Empty<IValidator<TestRequest>>());
            var nextWasCalled = false;

            RequestHandlerDelegate<TestResponse> next = _ =>
            {
                nextWasCalled = true;
                return Task.FromResult(expectedResponse);
            };

            var response = await behavior.Handle(request, next, CancellationToken.None);

            Assert.True(nextWasCalled);
            Assert.Same(expectedResponse, response);
        }

        [Fact]
        public async Task Handle_ShouldExecuteNext_WhenRequestIsValid()
        {
            var request = new TestRequest("Thiago");
            var expectedResponse = new TestResponse("Executado");
            var validator = new InlineValidator<TestRequest>();
            validator.RuleFor(value => value.Name).NotEmpty();

            var behavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { validator });
            var nextWasCalled = false;

            RequestHandlerDelegate<TestResponse> next = _ =>
            {
                nextWasCalled = true;
                return Task.FromResult(expectedResponse);
            };

            var response = await behavior.Handle(request, next, CancellationToken.None);

            Assert.True(nextWasCalled);
            Assert.Same(expectedResponse, response);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenRequestIsInvalid()
        {
            var request = new TestRequest(string.Empty);
            var validator = new InlineValidator<TestRequest>();
            validator.RuleFor(value => value.Name).NotEmpty().WithMessage("O nome é obrigatório.");

            var behavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { validator });
            var nextWasCalled = false;

            RequestHandlerDelegate<TestResponse> next = _ =>
            {
                nextWasCalled = true;
                return Task.FromResult(new TestResponse("Executado"));
            };

            var exception = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));

            Assert.False(nextWasCalled);
            Assert.Single(exception.Errors);
            Assert.Equal(nameof(TestRequest.Name), exception.Errors.First().PropertyName);
            Assert.Equal("O nome é obrigatório.", exception.Errors.First().ErrorMessage);
        }

        private sealed record TestRequest(string Name) : IRequest<TestResponse>;

        private sealed record TestResponse(string Message);
    }
}
