using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessTracker.Models;

namespace FitnessTracker.Services
{
    public interface ITescoScraperService
    {
        Task<TescoProduct?> SearchProductAsync(string query);
        Task<IReadOnlyList<string>> SearchProductNamesAsync(string query);
    }
}