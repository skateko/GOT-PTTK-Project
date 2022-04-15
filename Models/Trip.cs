using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class Trip
    {
        public int TripId { get; set; }
        [Required]
        public string? StartDate { get; set; }
        [Required]
        public string? EndDate { get; set; }
        [Required]
        public int? Points { get; set; }
    }
}
