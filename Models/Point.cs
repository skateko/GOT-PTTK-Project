using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class Point
    {
        public int PointId { get; set; }
        [Required]
        [MaxLength(40)]
        public string? PointName { get; set; }

        public ICollection<Stage>? StagesOpening { get; set; }
        public ICollection<Stage>? StagesClosing { get; set; }
    }
}
