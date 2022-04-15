using System.ComponentModel.DataAnnotations;

namespace GOTHelperEng.Models
{
    public class Gender
    {
        public int GenderId { get; set; }

        [Required]
        public string? GenderName { get; set; }
    }
}
