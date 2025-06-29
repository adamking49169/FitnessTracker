using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
    public class ExerciseDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("name_original")]
        public string NameOriginal { get; set; } = string.Empty;
    }
}
