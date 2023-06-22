namespace Roles.Models.ViewModel
{
    public class RolePermissionsViewModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public Dictionary<string, bool>? Permissions { get; set; } // Dictionary to store table/resource and its permission status
        public Dictionary<string, bool>? ReadPermissions { get; set; }
        public Dictionary<string, bool>? AddPermissions { get; set; }
        public Dictionary<string, bool>? EditPermissions { get; set; }
        public Dictionary<string, bool>? DeletePermissions { get; set; }
    }

}
