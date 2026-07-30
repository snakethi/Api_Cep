using ApiCep.Application.Address.Models;

namespace ApiCep.Application.Interfaces.ExternalServices
{
    public interface IViaCepService
    {
        Task<ViaCepAddressResult?> GetAddressAsync(string zipCode, CancellationToken cancellationToken = default);
    }
}
