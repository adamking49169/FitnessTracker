using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWgerService _wger;
        private readonly ICalorieService _calories;

        public WorkoutsController(
            ApplicationDbContext context,
            IWgerService wgerService,
            ICalorieService calorieService)
        {
            _context = context;
            _wger = wgerService;
            _calories = calorieService;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = _context.Workouts
                                 .Where(w => w.UserId == userId);
            return View(await items.ToListAsync());
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound();

            return View(workout);
        }

        // GET: Workouts/Create
        public async Task<IActionResult> Create()
        {
            // Pre-fill userId and date
            var model = new Workout
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Date = DateTime.Today,
                UserWeightKg = 0 // or pull from user profile
            };

            // Load exercise list from wger
            var exercises = await _wger.GetExercisesAsync();
            ViewBag.Exercises = new SelectList(exercises, "Name", "Name");

            return View(model);
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workout workout)
        {
            // Re-assign userId and date to prevent tampering
            workout.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            workout.Date = DateTime.Today;

            if (!ModelState.IsValid)
            {
                // re-populate dropdown on error
                var exercises = await _wger.GetExercisesAsync();
                ViewBag.Exercises = new SelectList(exercises, "Name", "Name", workout.Name);
                return View(workout);
            }

            // Fetch calories burned
            workout.CaloriesBurned = await _calories
                .GetCaloriesBurnedAsync(
                    workout.Name,
                    workout.UserWeightKg,
                    workout.DurationMinutes);

            _context.Add(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound();

            // populate dropdown, selecting current exercise
            var exercises = await _wger.GetExercisesAsync();
            ViewBag.Exercises = new SelectList(exercises, "Name", "Name", workout.Name);

            return View(workout);
        }

        // POST: Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Workout workout)
        {
            if (id != workout.Id)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exists = await _context.Workouts
                .AsNoTracking()
                .AnyAsync(w => w.Id == id && w.UserId == userId);
            if (!exists)
                return NotFound();

            workout.UserId = userId;
            workout.Date = workout.Date.Date;

            if (!ModelState.IsValid)
            {
                var exercises = await _wger.GetExercisesAsync();
                ViewBag.Exercises = new SelectList(exercises, "Name", "Name", workout.Name);
                return View(workout);
            }

            workout.CaloriesBurned = await _calories
                .GetCaloriesBurnedAsync(
                    workout.Name,
                    workout.UserWeightKg,
                    workout.DurationMinutes);

            try
            {
                _context.Update(workout);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var stillExists = await _context.Workouts
                    .AnyAsync(w => w.Id == id && w.UserId == userId);
                if (!stillExists)
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound();

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null)
                return NotFound();

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
