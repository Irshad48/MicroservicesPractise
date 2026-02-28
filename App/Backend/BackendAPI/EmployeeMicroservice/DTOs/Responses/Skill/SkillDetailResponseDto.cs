using EmployeeMicroservice.DTOs.Responses.Employee;

namespace EmployeeMicroservice.DTOs.Responses.Skill
{
    public class SkillDetailResponseDto : SkillResponseDto
    {
        public int EmployeeCount { get; set; }

        public List<EmployeeSkillSummaryDto> Employees { get; set; } = new();
    }

    public class EmployeeSkillSummaryDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string ProficiencyLevel { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsPrimarySkill { get; set; }
    }
}