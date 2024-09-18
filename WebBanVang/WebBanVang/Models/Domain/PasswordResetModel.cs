using System.ComponentModel.DataAnnotations;

namespace WebBanVang.Models.Domain
{
    public class PasswordResetModel
    {
        public string Token { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least 8 characters long", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[a-z])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one number, and one character")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "Confirm Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
