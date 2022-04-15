using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class MountainRange
    {
        public int MountainRangeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string? MountainRangeName { get; set; }
        [Required]
        [MaxLength(5)]
        public string? MountainRangeAbbr { get; set; }
        [Required]
        public int? MountainAreaId { get; set; }
        public MountainArea? MountainArea { get; set; }

    }
}
