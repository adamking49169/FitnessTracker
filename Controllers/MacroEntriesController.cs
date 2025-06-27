using FitnessTracker.Data;
using FitnessTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessTracker.Controllers
{
    public class MacroEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MacroEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [Authorize]
        public async Task<IActionResult> Today()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // use local Date rather than Utc, if your form is saving local-date values
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var entries = await _context.MacroEntries
                .Where(m => m.UserId == userId
                         && m.Date >= today
                         && m.Date < tomorrow)
                .ToListAsync();

            ViewBag.Summary = new
            {
                Carbs = entries.Sum(e => e.CarbsGrams),
                Protein = entries.Sum(e => e.ProteinGrams),
                Fat = entries.Sum(e => e.FatGrams),
                Calories = entries.Sum(e => e.CarbsGrams * 4
                                       + e.ProteinGrams * 4
                                       + e.FatGrams * 9)
            };

            return View(entries);
        }

        // GET: MacroEntries
        public async Task<IActionResult> Index()
        {
            return View(await _context.MacroEntries.ToListAsync());
        }

        // GET: MacroEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var macroEntry = await _context.MacroEntries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (macroEntry == null)
            {
                return NotFound();
            }

            return View(macroEntry);
        }

        // GET: MacroEntries/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new MacroEntry
            {
                UserId = userId,
                Date = DateTime.Today
            };
            return View(model);
        }

        // POST: MacroEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Date,CarbsGrams,ProteinGrams,FatGrams")] MacroEntry macroEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(macroEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(macroEntry);
        }

        // GET: MacroEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var macroEntry = await _context.MacroEntries.FindAsync(id);
            if (macroEntry == null)
            {
                return NotFound();
            }
            return View(macroEntry);
        }

        // POST: MacroEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Date,CarbsGrams,ProteinGrams,FatGrams")] MacroEntry macroEntry)
        {
            if (id != macroEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(macroEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MacroEntryExists(macroEntry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(macroEntry);
        }

        // GET: MacroEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var macroEntry = await _context.MacroEntries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (macroEntry == null)
            {
                return NotFound();
            }

            return View(macroEntry);
        }

        // POST: MacroEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var macroEntry = await _context.MacroEntries.FindAsync(id);
            if (macroEntry != null)
            {
                _context.MacroEntries.Remove(macroEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MacroEntryExists(int id)
        {
            return _context.MacroEntries.Any(e => e.Id == id);
        }
    }
}
