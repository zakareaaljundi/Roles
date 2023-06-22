using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Roles.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Position { get; set; }
        public int Age { get; set; }
        public decimal? Salary { get; set; }
        [ForeignKey("Department")]
        [Display(Name = "Department Name")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
