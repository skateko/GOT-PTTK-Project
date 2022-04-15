using System.ComponentModel.DataAnnotations;

namespace GOTHelperEng.Models
{
    public class Tourist
    {
        public int TouristId { get; set; }
        [Required]
        public int? TouristNumber { get; set; }
        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
