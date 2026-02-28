namespace EmployeeMicroservice.DTOs.Responses.EmployeeSkill
{
    public class EmployeeSkillDetailDto
    {
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? ProficiencyLevel { get; set; }
        public int YearsOfExperience { get; set; }
        public bool IsPrimarySkill { get; set; }
    }
}
