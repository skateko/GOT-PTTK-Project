using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.ViewModels
{
    public class TripViewModel
    {
        public int TripId { get; set; }
        [Required]
        public string? StartDate { get; set; }
        [Required]
        public string? EndDate { get; set; }
    }
}
