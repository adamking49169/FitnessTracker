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
            var url = $"cgi/search.pl?search_terms={Uri.EscapeDataString(query)}&search_simple=1&action=process&page_size=20&json=1&lc=en";
            try
            {
                using var res = await _client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                    return null;

                var json = await res.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<OffSearchResponse>(json);
                if (data?.Products == null || data.Products.Count == 0)
                    return null;

                var normalizedQuery = query.ToLowerInvariant();
                var best = data.Products
                    .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                    .OrderBy(p => LevenshteinDistance(normalizedQuery, p.Name!.ToLowerInvariant()))
                    .FirstOrDefault();
                return best ?? data.Products.FirstOrDefault();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
            {
                Console.Error.WriteLine($"OFF error: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<OffProduct>> SearchProductsAsync(string query, int limit = 5)
        {
            var url = $"cgi/search.pl?search_terms={Uri.EscapeDataString(query)}&search_simple=1&action=process&page_size=20&json=1&lc=en";
            try
            {
                using var res = await _client.GetAsync(url);
                if (!res.IsSuccessStatusCode)
                    return Enumerable.Empty<OffProduct>();

                var json = await res.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<OffSearchResponse>(json);
                if (data?.Products == null || data.Products.Count == 0)
                    return Enumerable.Empty<OffProduct>();

                var normalizedQuery = query.ToLowerInvariant();
                var results = data.Products
                    .Where(p => !string.IsNullOrWhiteSpace(p.Name)
                                && p.Nutriments != null
                                && p.Nutriments.EnergyKcal100g.HasValue
                                && p.Nutriments.Proteins100g.HasValue
                                && p.Nutriments.Carbs100g.HasValue
                                && p.Nutriments.Fat100g.HasValue)
                    .OrderBy(p => LevenshteinDistance(normalizedQuery, p.Name!.ToLowerInvariant()))
                    .Take(limit)
                    .ToList();

                return results;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is JsonException)
            {
                Console.Error.WriteLine($"OFF error: {ex.Message}");
                return Enumerable.Empty<OffProduct>();
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
        private static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
                return t?.Length ?? 0;
            if (string.IsNullOrEmpty(t))
                return s.Length;

            var d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }
    }
}