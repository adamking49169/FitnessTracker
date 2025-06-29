using System.Threading.Tasks;

namespace FitnessTracker.Services
{
    public interface ICalorieService
    {
        /// <summary>
        /// Returns estimated calories burned for an activity name, weight (kg), and duration (min).
        /// </summary>
        Task<int> GetCaloriesBurnedAsync(string activity, int weightKg, int durationMin);
        Task<IEnumerable<string>> GetActivitiesAsync();
    }
}
