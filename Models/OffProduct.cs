using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
    public class OffSearchResponse
    {
        [JsonPropertyName("products")]
        public List<OffProduct> Products { get; set; } = new();
    }

    public class OffProduct
    {
        [JsonPropertyName("product_name")]
        public string? Name { get; set; }

        [JsonPropertyName("nutriments")]
        public OffNutriments? Nutriments { get; set; }
    }

    public class OffNutriments
    {
        [JsonPropertyName("energy-kcal_100g")]
        public double? EnergyKcal100g { get; set; }

        [JsonPropertyName("proteins_100g")]
        public double? Proteins100g { get; set; }

        [JsonPropertyName("carbohydrates_100g")]
        public double? Carbs100g { get; set; }

        [JsonPropertyName("fat_100g")]
        public double? Fat100g { get; set; }
    }
}
