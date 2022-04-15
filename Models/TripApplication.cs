using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class TripApplication
    {
        [Key]
        public int TripApplicationId { get; set; }
        [Required]
        public string? CreationDate { get; set; }
        [Required]
        public bool? IsApproved { get; set; }
        [Required]
        public int? TripId { get; set; }
        public Trip? Trip { get; set; }
        [Required]
        public int? BookletId { get; set; }
        public Booklet? Ksiazeczka { get; set; }
    }
}
