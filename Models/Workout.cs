namespace FitnessTracker.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }

        // instead of Name:
        public int ExerciseId { get; set; }
        public ExerciseDto? Exercise { get; set; }

        public int DurationMinutes { get; set; }
        public int UserWeightKg { get; set; }
        public int CaloriesBurned { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? ExerciseName { get; set; }
    }
}
