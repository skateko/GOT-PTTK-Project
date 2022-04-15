using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GOTHelperEng.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        public int? GenderId { get; set; }
        public Gender? Gender { get; set; }
    }
}
