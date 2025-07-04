using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Services
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        private readonly HttpClient _client;

        public OpenFoodFactsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<OffProduct?> SearchProductAsync(string query)
        {
            var url = $"cgi/search.pl?search_terms={Uri.EscapeDataString(query)}&search_simple=1&action=process&page_size=1&json=1&lc=en";
            try
            {
                using var res = await _client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                    return null;

                var json = await res.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<OffSearchResponse>(json);
                return data?.Products?.FirstOrDefault();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
            {
                Console.Error.WriteLine($"OFF error: {ex.Message}");
                return null;
            }
        }
        public async Task<OffProduct?> GetProductByBarcodeAsync(string barcode)
        {
            var url = $"api/v0/product/{Uri.EscapeDataString(barcode)}.json";
            try
            {
                using var res = await _client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                    return null;

                var json = await res.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("product", out var prodEl))
                {
                    return prodEl.Deserialize<OffProduct>();
                }
                return null;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
            {
                Console.Error.WriteLine($"OFF error: {ex.Message}");
                return null;
            }
        }
    }
}