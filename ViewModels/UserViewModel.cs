using System.ComponentModel.DataAnnotations;
using GOTHelperEng.Models;
using Microsoft.AspNetCore.Identity;

namespace GOTHelperEng.ViewModels
{
    public class UserViewModel
    {

        public string? Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string? Surname { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public int? GenderId { get; set; }

        public Gender? Gender { get; set; }
        
        [Required]
        public string? RoleName { get; set; }
    }
}
