using System.Collections.Generic;

namespace FitnessTracker.Models
{
    public class WgerResponse<T>
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
        public List<T> Results { get; set; } = new();
    }
}
