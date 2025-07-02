using System.Threading.Tasks;
using FitnessTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TescoController : ControllerBase
    {
        private readonly ITescoScraperService _tesco;
        public TescoController(ITescoScraperService tesco)
        {
            _tesco = tesco;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");
            var product = await _tesco.SearchProductAsync(query);
            if (product == null)
                return NotFound("Product not found.");
            return Ok(product);
        }

        [HttpGet("list")]
        public async Task<IActionResult> List(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");
            var list = await _tesco.SearchProductNamesAsync(query);
            return Ok(list);
        }
    }
}