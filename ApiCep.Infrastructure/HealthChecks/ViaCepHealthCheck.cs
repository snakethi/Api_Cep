using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace ApiCep.Infrastructure.HealthChecks
{
    public sealed class ViaCepHealthCheck : IHealthCheck
    {
        public const string HttpClientName = "ViaCepHealthCheck";

        private readonly IHttpClientFactory _httpClientFactory;

        public ViaCepHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient(HttpClientName);
                using var response = await httpClient.GetAsync("ws/01001000/json/", cancellationToken);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy("O ViaCEP está disponível.");

                return HealthCheckResult.Unhealthy($"O ViaCEP retornou o status {(int)response.StatusCode}.");
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                return HealthCheckResult.Unhealthy("Não foi possível acessar o ViaCEP.", exception);
            }
        }
    }
}
