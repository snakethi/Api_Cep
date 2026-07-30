using ApiCep.Infrastructure.ExternalServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text;


namespace ApiCep.Tests.Infrastructure.ExternalServices
{
    public sealed class ViaCepServiceTests
    {
        [Fact]
        public async Task GetAddressAsync_ShouldReturnAddress_WhenZipCodeExists()
        {
            const string json = """
        {
          "cep": "01310-100",
          "logradouro": "Avenida Paulista",
          "complemento": "",
          "bairro": "Bela Vista",
          "localidade": "São Paulo",
          "uf": "SP",
          "erro": false
        }
        """;

            var handler = new TestHttpMessageHandler((request, _) =>
            {
                Assert.Equal("https://viacep.com.br/ws/01310100/json/", request.RequestUri?.ToString());

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            });

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://viacep.com.br/")
            };

            using var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ViaCepService(httpClient, cache, NullLogger<ViaCepService>.Instance);

            var result = await service.GetAddressAsync("01310-100", CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("01310100", result.ZipCode);
            Assert.Equal("Avenida Paulista", result.Street);
            Assert.Equal("Bela Vista", result.Neighborhood);
            Assert.Equal("São Paulo", result.City);
            Assert.Equal("SP", result.State);
            Assert.Equal(1, handler.RequestCount);
        }

        [Fact]
        public async Task GetAddressAsync_ShouldReturnNull_WhenZipCodeDoesNotExist()
        {
            const string json = """
        {
          "erro": true
        }
        """;

            var handler = new TestHttpMessageHandler((_, _) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            });

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://viacep.com.br/")
            };

            using var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ViaCepService(httpClient, cache, NullLogger<ViaCepService>.Instance);

            var result = await service.GetAddressAsync("00000-000", CancellationToken.None);

            Assert.Null(result);
            Assert.Equal(1, handler.RequestCount);
        }

        [Fact]
        public async Task GetAddressAsync_ShouldUseCache_WhenZipCodeWasAlreadyRequested()
        {
            const string json = """
        {
          "cep": "01310-100",
          "logradouro": "Avenida Paulista",
          "complemento": "",
          "bairro": "Bela Vista",
          "localidade": "São Paulo",
          "uf": "SP",
          "erro": false
        }
        """;

            var handler = new TestHttpMessageHandler((_, _) =>
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            });

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://viacep.com.br/")
            };

            using var cache = new MemoryCache(new MemoryCacheOptions());
            var service = new ViaCepService(httpClient, cache, NullLogger<ViaCepService>.Instance);

            var firstResult = await service.GetAddressAsync("01310-100", CancellationToken.None);
            var secondResult = await service.GetAddressAsync("01310100", CancellationToken.None);

            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);
            Assert.Equal(firstResult.ZipCode, secondResult.ZipCode);
            Assert.Equal(firstResult.Street, secondResult.Street);
            Assert.Equal(1, handler.RequestCount);
        }

        private sealed class TestHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responseFactory;

            public TestHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory)
            {
                _responseFactory = responseFactory;
            }

            public int RequestCount { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestCount++;
                return _responseFactory(request, cancellationToken);
            }
        }
    }
}
