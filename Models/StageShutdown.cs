using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class StageShutdown
    {
        [Key]
        public int ShutdownId { get; set; }
        [Required]
        [MaxLength(50)]
        public string? StartDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string? EndDate { get; set; }
        [MaxLength(5000)]
        public string? Opis { get; set; }
        [Required]
        public int? StageId { get; set; }
        public Stage? Stage { get; set; }
    }
}
