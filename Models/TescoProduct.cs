using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
    public class TescoProduct
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("energyKcal100g")]
        public double? EnergyKcal100g { get; set; }

        [JsonPropertyName("carbs100g")]
        public double? Carbs100g { get; set; }

        [JsonPropertyName("fat100g")]
        public double? Fat100g { get; set; }

        [JsonPropertyName("protein100g")]
        public double? Protein100g { get; set; }
    }
}