using System.ComponentModel.DataAnnotations;



namespace GOTHelperEng.Models
{
    public class MountainArea
    {
        public int MountainAreaId { get; set; }
        [Required]
        [MaxLength(30)]
        public string? MoutainAreaName { get; set; }
    }
}
