using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
    public class OffSearchResponse
    {
        [JsonPropertyName("products")]
        public List<OffProduct> Products { get; set; } = new();
    }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public class OffProduct
    {
        [JsonPropertyName("product_name")]
        public string? Name { get; set; }

        [JsonPropertyName("nutriments")]
        public OffNutriments? Nutriments { get; set; }
    }

    public class OffNutriments
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("energy-kcal_100g")]
        public double? EnergyKcal100g { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("proteins_100g")]
        public double? Proteins100g { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("carbohydrates_100g")]
        public double? Carbs100g { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        [JsonPropertyName("fat_100g")]
        public double? Fat100g { get; set; }
    }
}