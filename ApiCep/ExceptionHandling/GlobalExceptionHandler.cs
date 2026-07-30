using ApiCep.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiCep.Api.ExceptionHandling
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
        {

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).Distinct().ToArray());

                var validationProblemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Erro de validação",
                    Detail = "Um ou mais campos informados são inválidos.",
                    Instance = httpContext.Request.Path
                };

                validationProblemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

                _logger.LogWarning(
                    exception,
                    "Erro de validação ao processar a requisição. TraceId: {TraceId}",
                    httpContext.TraceIdentifier);

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);

                return true;
            }


            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ConflictException => StatusCodes.Status409Conflict,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode >= StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Erro inesperado ao processar a requisição. TraceId: {TraceId}", httpContext.TraceIdentifier);
            else
                _logger.LogWarning(exception, "Erro de negócio ao processar a requisição. TraceId: {TraceId}", httpContext.TraceIdentifier);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "Ocorreu um erro interno ao processar a solicitação."
                    : exception.Message,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static string GetTitle(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status404NotFound => "Recurso não encontrado",
                StatusCodes.Status401Unauthorized => "Não autorizado",
                StatusCodes.Status400BadRequest => "Requisição inválida",
                StatusCodes.Status409Conflict => "Conflito",
                _ => "Erro interno do servidor"
            };
        }
    }
}
