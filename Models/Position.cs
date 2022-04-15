using System.ComponentModel.DataAnnotations;

namespace GOTHelperEng.Models
{
    public class Position
    {
        public int PositionId { get; set; }
        [Required]
        [MaxLength(3)]
        public int? StageNumber { get; set; }
        [Required]
        public bool? Direction { get; set; }
        [Required]
        public int? TripId { get; set; }
        [Required]
        public Trip? Trip { get; set; }
        [Required]
        public int? StageId { get; set; }
        public Stage? Stage { get; set; }
    }
}