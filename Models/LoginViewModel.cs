using System.ComponentModel.DataAnnotations;

namespace ECommercePlatform.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // Initialize with default value

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Initialize with default value

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}