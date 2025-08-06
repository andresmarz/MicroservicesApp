using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ordering.Application.DTOs.External;
using Ordering.Application.Interfaces;

namespace Ordering.Infrastructure.HttpClients
{
    public class CatalogServiceHttpClient : ICatalogServiceHttpClient
    {
        private readonly HttpClient _httpClient;

        public CatalogServiceHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(Guid productId)
        {
            var response = await _httpClient.GetAsync($"/api/external-product/{productId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        }
    }
}