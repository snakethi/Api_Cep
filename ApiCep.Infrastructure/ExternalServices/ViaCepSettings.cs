namespace ApiCep.Infrastructure.ExternalServices
{
    public sealed class ViaCepSettings
    {
        public const string SectionName = "ViaCep";

        public string BaseUrl { get; init; } = string.Empty;
    }
}
