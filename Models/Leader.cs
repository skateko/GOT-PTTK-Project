using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class Leader
    {
        public int LeaderId { get; set; }
        [Required]
        public string? HireDate { get; set; }
        public string? FireDate { get; set; }
        [Required]
        public int? LeaderNumber { get; set; }
        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }

    }
}
