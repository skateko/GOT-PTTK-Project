using System.ComponentModel.DataAnnotations;
using GOTHelperEng.Models;

namespace GOTHelperEng.ViewModels
{
    public class TripApplicationViewModel
    {
        [Key]
        public int TripApplicationId { get; set; }
        [Required]
        public string? TripStartDate { get; set; }
        [Required]
        public string? TripEndDate { get; set; }
        [Required]
        public int? BookletId { get; set; }
        public Booklet? Ksiazeczka { get; set; }
    }
}
