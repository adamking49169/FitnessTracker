using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Models
{
    public class MacroEntry
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public int CarbsGrams { get; set; }
        public int ProteinGrams { get; set; }
        public int FatGrams { get; set; }
        [NotMapped]
        public int Calories => CarbsGrams * 4 + ProteinGrams * 4 + FatGrams * 9;
    }
}
