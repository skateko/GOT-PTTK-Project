using System.ComponentModel.DataAnnotations;
namespace GOTHelperEng.Models
{
    public class Booklet
    {
        public int BookletId { get; set; }
        [Required]
        public int? TouristId { get; set; }
        public Tourist? Tourist { get; set; }

        public string? CreationDate { get; set; }




    }
}
