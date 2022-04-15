using System.ComponentModel.DataAnnotations;

namespace GOTHelperEng.Models
{
    public class Stage
    {
        public int StageId { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a value bigger or equal {0}")]
        public double? Length { get; set; }
        [Required]
        [Range(0, 50, ErrorMessage = "Please enter a value bigger or equal {0}")]
        public int? PointsForwards { get; set; }
        [Range(0, 50, ErrorMessage = "Please enter a value bigger or equal {0}")]
        public int? PointsBackwards { get; set; }
        [MaxLength(5000)]
        public string? RouteDescription { get; set; }
        [Required]
        public int? MountainRangeId { get; set; }
        public MountainRange? MountainRange { get; set; }
        [Required]
        public int? StartPointId { get; set; }
        public Point? StartPoint { get; set; }
        [Required]
        public int? EndPointId { get; set; }
        public Point? EndPoint { get; set; }
        public int? TouristId { get; set; }
        public Tourist? Tourist { get; set; }
    }
}
