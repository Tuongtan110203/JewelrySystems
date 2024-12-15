using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.Domain
{
    public class PasswordResetRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
