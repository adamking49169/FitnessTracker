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

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest();

            var product = await _off.SearchProductAsync(query);
            if (product == null || product.Nutriments == null)
                return NotFound();

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
