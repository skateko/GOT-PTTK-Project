using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class Level
    {
        public int LevelId { get; set; }
        [Required]
        [MaxLength(20)]
        public string? LevelName { get; set; }
    }
}
