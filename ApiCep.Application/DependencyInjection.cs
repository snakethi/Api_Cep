using ApiCep.Application.Common.Behaviors;
using ApiCep.Application.User.Commands.CreateUser;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ApiCep.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
