using System.ComponentModel.DataAnnotations;

namespace Roles.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter Email")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
