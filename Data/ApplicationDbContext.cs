using FitnessTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<MacroEntry> MacroEntries { get; set; }
        public DbSet<ExerciseDto> Exercises { get; set; }
    }
}
