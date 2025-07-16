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
        /// <summary>
        /// Returns a list of products matching the query ordered by similarity.
        /// Only products with all required nutrient fields are returned.
        /// </summary>
        Task<IEnumerable<OffProduct>> SearchProductsAsync(string query, int limit = 5);
        Task<OffProduct?> GetProductByBarcodeAsync(string barcode);
    }
}