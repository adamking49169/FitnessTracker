using System.Threading.Tasks;
using FitnessTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenFoodController : ControllerBase
    {
        private readonly IOpenFoodFactsService _off;

        public OpenFoodController(IOpenFoodFactsService offService)
        {
            _off = offService;
        }

        // 🔍 Search by food name
        [HttpGet("search")]
        public async Task<IActionResult> Search(string query, double? grams)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var product = await _off.SearchProductAsync(query);
            if (product == null || product.Nutriments == null)
                return NotFound("Food not found.");


            double factor = grams.HasValue && grams > 0 ? grams.Value / 100.0 : 1;

            return Ok(new
            {
                name = product.Name,
                grams = grams ?? 100,
                calories = product.Nutriments.EnergyKcal100g * factor,
                protein = product.Nutriments.Proteins100g * factor,
                carbs = product.Nutriments.Carbs100g * factor,
                fat = product.Nutriments.Fat100g * factor
            });
        }

        // 🧾 Sample/test response
        [HttpGet("example")]
        public IActionResult Example(double? grams)
        {
            double factor = grams.HasValue && grams > 0 ? grams.Value / 100.0 : 1;
            return Ok(new
            {
                name = "Banana",
                grams = grams ?? 100,
                calories = 89 * factor,
                protein = 1.1 * factor,
                carbs = 22.8 * factor,
                fat = 0.3 * factor
            });
        }

        // 📦 Search by barcode/UPC
        [HttpGet("barcode/{code}")]
        public async Task<IActionResult> GetByBarcode(string code, double? grams)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 13)
                return BadRequest("A 13-digit barcode is required.");

            var product = await _off.GetProductByBarcodeAsync(code);
            if (product == null || product.Nutriments == null)
                return NotFound("Product not found.");
            double factor = grams.HasValue && grams > 0 ? grams.Value / 100.0 : 1;
            return Ok(new
            {
                name = product.Name,
                barcode = code,
                grams = grams ?? 100,
                calories = product.Nutriments.EnergyKcal100g * factor,
                protein = product.Nutriments.Proteins100g * factor,
                carbs = product.Nutriments.Carbs100g * factor,
                fat = product.Nutriments.Fat100g * factor
            });
        }

        // ⚗️ Return full nutrient object for advanced users
        [HttpGet("nutrients/{query}")]
        public async Task<IActionResult> GetNutrientValues(string query, double? grams)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var product = await _off.SearchProductAsync(query);
            if (product == null || product.Nutriments == null)
                return NotFound("Food not found.");
            double factor = grams.HasValue && grams > 0 ? grams.Value / 100.0 : 1;
            return Ok(new
            {
                name = product.Name,
                grams = grams ?? 100,
                calories = product.Nutriments.EnergyKcal100g * factor,
                protein = product.Nutriments.Proteins100g * factor,
                carbs = product.Nutriments.Carbs100g * factor,
                fat = product.Nutriments.Fat100g * factor
            });
        }
    }
}
