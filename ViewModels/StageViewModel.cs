using System.ComponentModel.DataAnnotations;
using GOTHelperEng.Models;

namespace GOTHelperEng.ViewModels
{
    public class StageViewModel
    {
        public int StageId { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a value bigger or equal {0}")]
        public double? LengthInMeters { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a value bigger or equal {0}")]
        public double? HeightDifference { get; set; }
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
    }
}
