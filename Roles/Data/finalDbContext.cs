using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Roles.Models;

namespace Roles.Data
{
    public class finalDbContext : IdentityDbContext
    {
        public finalDbContext(DbContextOptions<finalDbContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
    }
}