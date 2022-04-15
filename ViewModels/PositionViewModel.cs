using System.ComponentModel.DataAnnotations;
using GOTHelperEng.Models; 

namespace GOTHelperEng.ViewModels
{
    public class PositionViewModel
    {
        public int PositionId { get; set; }
        [Required]
        public bool? Direction { get; set; }
        [Required]
        public int? StageId { get; set; }
        public Stage? Stage { get; set; }
    }
}