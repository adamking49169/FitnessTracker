using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
    public class WgerResponse<T>
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("next")]
        public string? Next { get; set; }
        [JsonPropertyName("previous")]
        public string? Previous { get; set; }
        [JsonPropertyName("results")]
        public List<T> Results { get; set; } = new();
    }
}
