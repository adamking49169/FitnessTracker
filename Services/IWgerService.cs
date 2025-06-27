using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Services
{
    public interface IWgerService
    {
        Task<IEnumerable<ExerciseDto>> GetExercisesAsync(int page = 1);
    }
}
