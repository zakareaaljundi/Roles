using System.ComponentModel.DataAnnotations;

namespace Roles.Models.ViewModel
{
    public class CreateRoleViewModel
    {
        [Required]
        public string? RoleName { get; set; }
    }
}
