using EmployeeMicroservice.Models.Entities;

namespace EmployeeMicroservice.Models
{
    public class Employee
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //navigation properties
        public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; }
    }
}
