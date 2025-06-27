namespace FitnessTracker.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public int UserWeightKg { get; set; }
    }
}
