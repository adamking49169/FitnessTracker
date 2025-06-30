using System.Threading.Tasks;
using FitnessTracker.Models;
using System.Collections.Generic;

namespace FitnessTracker.Services
{
    public interface IOpenFoodFactsService
    {
        /// <summary>
        /// Searches for a product by query and returns the first result's nutriments.
        /// </summary>
        Task<OffProduct?> SearchProductAsync(string query);
    }
}