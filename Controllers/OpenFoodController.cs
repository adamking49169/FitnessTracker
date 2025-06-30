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
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var product = await _off.SearchProductAsync(query);
            if (product == null || product.Nutriments == null)
                return NotFound("Food not found.");

            return Ok(new
            {
                name = product.Name,
                caloriesPer100g = product.Nutriments.EnergyKcal100g,
                proteinPer100g = product.Nutriments.Proteins100g,
                carbsPer100g = product.Nutriments.Carbs100g,
                fatPer100g = product.Nutriments.Fat100g
            });
        }

        // 🧾 Sample/test response
        [HttpGet("example")]
        public IActionResult Example()
        {
            return Ok(new
            {
                name = "Banana",
                caloriesPer100g = 89,
                proteinPer100g = 1.1,
                carbsPer100g = 22.8,
                fatPer100g = 0.3
            });
        }

        // 📦 Search by barcode/UPC
        [HttpGet("barcode/{code}")]
        public async Task<IActionResult> GetByBarcode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Barcode is required.");

            var product = await _off.GetProductByBarcodeAsync(code);
            if (product == null || product.Nutriments == null)
                return NotFound("Product not found.");

            return Ok(new
            {
                name = product.Name,
                barcode = code,
                caloriesPer100g = product.Nutriments.EnergyKcal100g,
                proteinPer100g = product.Nutriments.Proteins100g,
                carbsPer100g = product.Nutriments.Carbs100g,
                fatPer100g = product.Nutriments.Fat100g
            });
        }

        // ⚗️ Return full nutrient object for advanced users
        [HttpGet("nutrients/{query}")]
        public async Task<IActionResult> GetNutrientValues(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var product = await _off.SearchProductAsync(query);
            if (product == null || product.Nutriments == null)
                return NotFound("Food not found.");
            return Ok(new
            {
                name = product.Name,
                caloriesPer100g = product.Nutriments.EnergyKcal100g,
                proteinPer100g = product.Nutriments.Proteins100g,
                carbsPer100g = product.Nutriments.Carbs100g,
                fatPer100g = product.Nutriments.Fat100g
            });
        }
    }
}
