using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FitnessTracker.Models
{
    public class ExerciseInfoDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("translations")]
        public List<ExerciseTranslationDto> Translations { get; set; } = new();
    }

    public class ExerciseTranslationDto
    {
        [JsonPropertyName("language")]
        public int Language { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}