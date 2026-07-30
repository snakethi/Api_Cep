using MediatR;
using System.Diagnostics;
using Microsoft.Extensions.Logging;


namespace ApiCep.Application.Common.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestName"] = requestName
            });

            _logger.LogInformation("Iniciando processamento de {RequestName}.", requestName);

            try
            {
                var response = await next();

                stopwatch.Stop();

                _logger.LogInformation("Processamento de {RequestName} concluído em {ElapsedMilliseconds} ms.", requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch
            {
                stopwatch.Stop();

                _logger.LogWarning("Processamento de {RequestName} falhou após {ElapsedMilliseconds} ms.", requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
