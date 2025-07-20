namespace FitnessTracker.Mobile.Models;

public class WorkoutDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ExerciseId { get; set; }
    public int DurationMinutes { get; set; }
    public int UserWeightKg { get; set; }
    public int CaloriesBurned { get; set; }
    public string? ExerciseName { get; set; }
}
