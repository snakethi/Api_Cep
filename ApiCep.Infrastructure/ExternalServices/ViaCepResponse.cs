
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ApiCep.Infrastructure.ExternalServices
{
    internal sealed class ViaCepResponse
    {
        [JsonPropertyName("cep")]
        public string? ZipCode { get; init; }

        [JsonPropertyName("logradouro")]
        public string? Street { get; init; }

        [JsonPropertyName("bairro")]
        public string? Neighborhood { get; init; }

        [JsonPropertyName("localidade")]
        public string? City { get; init; }

        [JsonPropertyName("uf")]
        public string? State { get; init; }

        [JsonPropertyName("erro")]
        public JsonElement? Error { get; init; }

        public bool IsNotFound()
        {
            if (!Error.HasValue)
                return false;

            return Error.Value.ValueKind == JsonValueKind.True || Error.Value.ValueKind == JsonValueKind.String && bool.TryParse(Error.Value.GetString(), out var error) && error;
        }
    }
}
