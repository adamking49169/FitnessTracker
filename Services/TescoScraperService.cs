using FitnessTracker.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace FitnessTracker.Services
{
    public class TescoScraperService : ITescoScraperService
    {
        private readonly HttpClient _client;
        public TescoScraperService(HttpClient client)
        {
            _client = client;
            if (!_client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                    "AppleWebKit/537.36 (KHTML, like Gecko) " +
                    "Chrome/123.0.0.0 Safari/537.36");
            }
        }

        public async Task<IReadOnlyList<string>> SearchProductNamesAsync(string query)
        {
            var url = $"https://www.tesco.com/groceries/en-GB/search?query={Uri.EscapeDataString(query)}";
            try
            {
                var html = await _client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes("//a[contains(@class,'tile-product__image-link')]");
                if (nodes == null)
                    return Array.Empty<string>();
                return nodes.Select(n => n.GetAttributeValue("aria-label", null) ?? n.InnerText.Trim())
                            .Where(n => !string.IsNullOrWhiteSpace(n))
                            .Distinct()
                            .ToList();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Console.Error.WriteLine($"Tesco scraper error: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public async Task<TescoProduct?> SearchProductAsync(string query)
        {
            var url = $"https://www.tesco.com/groceries/en-GB/search?query={Uri.EscapeDataString(query)}";
            try
            {
                var html = await _client.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var node = doc.DocumentNode.SelectSingleNode("//a[contains(@class,'tile-product__image-link')]");
                if (node == null)
                    return null;
                var link = node.GetAttributeValue("href", null);
                var name = node.GetAttributeValue("aria-label", null) ?? node.InnerText.Trim();
                if (string.IsNullOrEmpty(link))
                    return null;
                if (!link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    link = "https://www.tesco.com" + link;

                var prodHtml = await _client.GetStringAsync(link);
                var prodDoc = new HtmlDocument();
                prodDoc.LoadHtml(prodHtml);
                var table = prodDoc.DocumentNode.SelectSingleNode("//table[contains(@class,'nutrition')]");
                if (table == null)
                    return null;

                double? energy = null, carbs = null, fat = null, protein = null;
                foreach (var row in table.SelectNodes(".//tr") ?? Enumerable.Empty<HtmlNode>())
                {
                    var cells = row.SelectNodes("th|td");
                    if (cells == null || cells.Count < 2)
                        continue;
                    var key = cells[0].InnerText.Trim().ToLowerInvariant();
                    var valText = cells[1].InnerText.Trim().ToLowerInvariant();
                    valText = valText.Replace("kcal", string.Empty)
                                     .Replace("g", string.Empty)
                                     .Trim();
                    if (!double.TryParse(valText, out var val))
                        continue;
                    if (key.Contains("energy"))
                        energy = val;
                    else if (key.Contains("carbohydrate"))
                        carbs = val;
                    else if (key.Contains("fat"))
                        fat = val;
                    else if (key.Contains("protein"))
                        protein = val;
                }

                return new TescoProduct
                {
                    Name = name,
                    EnergyKcal100g = energy,
                    Carbs100g = carbs,
                    Fat100g = fat,
                    Protein100g = protein
                };
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                Console.Error.WriteLine($"Tesco scraper error: {ex.Message}");
                return null;
            }
        }
    }
}
