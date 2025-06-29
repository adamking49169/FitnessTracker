using FitnessTracker.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FitnessTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaloriesController : ControllerBase
    {
        private readonly ICalorieService _service;

        public CaloriesController(ICalorieService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string activity, int weightKg, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                return BadRequest("Activity is required");
            }
            if (weightKg <= 0 || durationMinutes <= 0)
            {
                return BadRequest("Weight and duration must be positive");
            }

            var calories = await _service.GetCaloriesBurnedAsync(activity, weightKg, durationMinutes);
            return Ok(new { calories });
        }
    }
}