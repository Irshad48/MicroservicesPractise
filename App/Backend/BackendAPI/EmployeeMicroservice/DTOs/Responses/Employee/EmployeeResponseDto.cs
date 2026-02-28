using EmployeeMicroservice.DTOs.Responses.EmployeeSkill;

namespace EmployeeMicroservice.DTOs.Responses.Employee
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Add this if you want skills included in employee response
        public List<EmployeeSkillResponseDto>? Skills { get; set; }
    }
}