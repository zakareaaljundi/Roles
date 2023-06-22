using System.ComponentModel.DataAnnotations;

namespace Roles.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter Email")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm And Password Not Match")]
        public string? ConfirmPassword { get; set; }
        public string? Phone { get; set; }
    }
}
