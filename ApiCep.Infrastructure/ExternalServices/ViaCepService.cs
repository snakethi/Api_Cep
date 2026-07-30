using ApiCep.Application.Address.Models;
using ApiCep.Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly.Timeout;
using System.Net.Http.Json;


namespace ApiCep.Infrastructure.ExternalServices
{
    public sealed class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ViaCepService> _logger;

        public ViaCepService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<ViaCepService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<ViaCepAddressResult?> GetAddressAsync(string zipCode, CancellationToken cancellationToken = default)
        {
            var normalizedZipCode = new string(zipCode.Where(char.IsDigit).ToArray());

            if (normalizedZipCode.Length != 8)
                throw new ArgumentException("O CEP deve possuir 8 dígitos.", nameof(zipCode));

            var cacheKey = $"viacep:{normalizedZipCode}";
            var fallbackCacheKey = $"viacep:fallback:{normalizedZipCode}";

            if (_memoryCache.TryGetValue(cacheKey, out ViaCepAddressResult? cachedAddress))
                return cachedAddress;

            try
            {
                var response = await _httpClient.GetAsync($"ws/{normalizedZipCode}/json/", cancellationToken);

                response.EnsureSuccessStatusCode();

                var viaCepResponse = await response.Content.ReadFromJsonAsync<ViaCepResponse>(cancellationToken: cancellationToken);

                if (viaCepResponse is null || viaCepResponse.IsNotFound())
                    return null;

                var result = new ViaCepAddressResult(normalizedZipCode, viaCepResponse.Street ?? string.Empty, viaCepResponse.Neighborhood ?? string.Empty, viaCepResponse.City ?? string.Empty, viaCepResponse.State ?? string.Empty);

                _memoryCache.Set(cacheKey, result, TimeSpan.FromHours(6));
                _memoryCache.Set(fallbackCacheKey, result, TimeSpan.FromDays(1));

                return result;
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or TimeoutRejectedException)
            {
                _logger.LogWarning(exception, "Falha ao consultar o ViaCEP para o CEP {ZipCode}.", normalizedZipCode);

                if (_memoryCache.TryGetValue(fallbackCacheKey, out ViaCepAddressResult? fallbackAddress))
                    return fallbackAddress;

                throw;
            }
        }
    }
}
