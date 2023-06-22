using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace Roles.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string? RoleId { get; set; }
        public string? TableName { get; set; }
        public bool ReadPermission { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool DeletePermission { get; set; }
    }
}