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
using Microsoft.AspNetCore.Authorization;

namespace FitnessTracker.Controllers
{
    [Authorize]
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWgerService _wger;
        private readonly ICalorieService _calories;

        public WorkoutsController(ApplicationDbContext context, IWgerService wgerService, ICalorieService calorieService)
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
                                .Where(w => w.UserId == userId)
                                .Include(w => w.Exercise);
            return View(await items.ToListAsync());
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .Include(w => w.Exercise)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null) return NotFound();
            return View(workout);
        }

        // GET: Workouts/Create
        public async Task<IActionResult> Create()
        {
            var model = new Workout
            {
                Date = DateTime.Now,
                UserWeightKg = 0
            };

            var wgerExercises = await _wger.GetExercisesAsync();
            var localNames = await _context.Exercises.Select(e => e.Name).ToListAsync();
            ViewBag.ExerciseNames = wgerExercises.Select(e => e.Name)
                                                 .Concat(localNames)
                                                 .Distinct()
                                                 .ToList();
            ViewBag.SelectedName = string.Empty;
            return View(model);
        }

        // POST: Workouts/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,DurationMinutes,UserWeightKg,ExerciseName")] Workout workout)
        {
            // Assign the logged-in user
            workout.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Remove any ModelState errors for UserId
            ModelState.Remove(nameof(workout.UserId));

            if (!ModelState.IsValid)
            {
                var wgerExercises = await _wger.GetExercisesAsync();
                var localNames = await _context.Exercises.Select(e => e.Name).ToListAsync();
                ViewBag.ExerciseNames = wgerExercises.Select(e => e.Name)
                                                      .Concat(localNames)
                                                      .Distinct()
                                                      .ToList();
                ViewBag.SelectedName = workout.ExerciseName;
                return View(workout);
            }

            // Lookup existing exercise by name; create if not found
            var exercise = (await _wger.GetExercisesAsync()).FirstOrDefault(e =>
               string.Equals(e.Name, workout.ExerciseName, StringComparison.OrdinalIgnoreCase));
            if (exercise == null)
            {
                var nextId = await _context.Exercises.AnyAsync()
                    ? await _context.Exercises.MaxAsync(e => e.Id) + 1
                    : 1;
                exercise = new ExerciseDto
                {
                    Id = nextId,
                    Name = workout.ExerciseName ?? string.Empty,
                    NameOriginal = workout.ExerciseName ?? string.Empty
                };
            }
            workout.ExerciseId = exercise.Id;
            // Ensure exercise exists in local DB
            if (await _context.Exercises.FindAsync(exercise.Id) == null)
            {
                using var tx = await _context.Database.BeginTransactionAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Exercises ON");
                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Exercises OFF");
                await tx.CommitAsync();
            }

            // Fetch calories burned
            workout.CaloriesBurned = await _calories.GetCaloriesBurnedAsync(
                exercise.Name,
                workout.UserWeightKg,
                workout.DurationMinutes);

            _context.Add(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null) return NotFound();

            var wgerExercises = await _wger.GetExercisesAsync();
            var dbExercises = await _context.Exercises.ToListAsync();
            ViewBag.ExerciseNames = wgerExercises.Select(e => e.Name)
                                                .Concat(dbExercises.Select(e => e.Name))
                                                .Distinct()
                                                .ToList();
            ViewBag.SelectedName = dbExercises.FirstOrDefault(e => e.Id == workout.ExerciseId)?.Name
                                   ?? wgerExercises.FirstOrDefault(e => e.Id == workout.ExerciseId)?.Name
                                   ?? string.Empty;
            return View(workout);
        }

        // POST: Workouts/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,DurationMinutes,UserWeightKg,ExerciseName")] Workout workout)
        {
            if (id != workout.Id) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await _context.Workouts.AsNoTracking().AnyAsync(w => w.Id == id && w.UserId == userId))
                return NotFound();

            // Assign user and clear ModelState error
            workout.UserId = userId;
            ModelState.Remove(nameof(workout.UserId));
            // Normalize date
            workout.Date = workout.Date.Date;

            if (!ModelState.IsValid)
            {
                var wgerExercises = await _wger.GetExercisesAsync();
                var dbNames = await _context.Exercises.Select(e => e.Name).ToListAsync();
                ViewBag.ExerciseNames = wgerExercises.Select(e => e.Name)
                                                    .Concat(dbNames)
                                                    .Distinct()
                                                    .ToList();
                ViewBag.SelectedName = workout.ExerciseName;
                return View(workout);
            }

            // Lookup existing exercise by name; create if not found
            var exercise2 = (await _wger.GetExercisesAsync()).FirstOrDefault(e =>
                 string.Equals(e.Name, workout.ExerciseName, StringComparison.OrdinalIgnoreCase));
            if (exercise2 == null)
            {
                var nextId = await _context.Exercises.AnyAsync()
                    ? await _context.Exercises.MaxAsync(e => e.Id) + 1
                    : 1;
                exercise2 = new ExerciseDto
                {
                    Id = nextId,
                    Name = workout.ExerciseName ?? string.Empty,
                    NameOriginal = workout.ExerciseName ?? string.Empty
                };
            }
            workout.ExerciseId = exercise2.Id;
            // Ensure exercise exists locally
            if (await _context.Exercises.FindAsync(exercise2.Id) == null)
            {
                using var tx = await _context.Database.BeginTransactionAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Exercises ON");
                _context.Exercises.Add(exercise2);
                await _context.SaveChangesAsync();
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Exercises OFF");
                await tx.CommitAsync();
            }

            // Fetch calories
            workout.CaloriesBurned = await _calories.GetCaloriesBurnedAsync(
                exercise2.Name,
                workout.UserWeightKg,
                workout.DurationMinutes);

            try
            {
                _context.Update(workout);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Workouts.AnyAsync(w => w.Id == id && w.UserId == userId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Workouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .Include(w => w.Exercise)
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null) return NotFound();
            return View(workout);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var workout = await _context.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (workout == null) return NotFound();

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
