using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FitnessTracker.Data;
using FitnessTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers.Api
{
    [ApiController]
    [Route("api/macroentries")]
    [Authorize]
    public class MacroEntriesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MacroEntriesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class MacroEntryDto
        {
            public int CarbsGrams { get; set; }
            public int ProteinGrams { get; set; }
            public int FatGrams { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry([FromBody] MacroEntryDto dto)
        {
            if (dto == null)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var entry = new MacroEntry
            {
                UserId = userId,
                Date = DateTime.Today,
                CarbsGrams = dto.CarbsGrams,
                ProteinGrams = dto.ProteinGrams,
                FatGrams = dto.FatGrams
            };

            _context.MacroEntries.Add(entry);
            await _context.SaveChangesAsync();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var entries = await _context.MacroEntries
                .Where(e => e.UserId == userId && e.Date >= today && e.Date < tomorrow)
                .ToListAsync();
            var summary = new
            {
                carbs = entries.Sum(e => e.CarbsGrams),
                protein = entries.Sum(e => e.ProteinGrams),
                fat = entries.Sum(e => e.FatGrams),
                calories = entries.Sum(e => e.CarbsGrams * 4 + e.ProteinGrams * 4 + e.FatGrams * 9)
            };

            return Ok(summary);
        }
    }
}