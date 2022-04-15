using System.ComponentModel.DataAnnotations;


namespace GOTHelperEng.Models
{
    public class Administrator
    {
        public int AdministratorId { get; set; }
        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
