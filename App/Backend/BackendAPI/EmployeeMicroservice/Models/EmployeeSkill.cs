using EmployeeMicroservice.Models.Enums;  // Add this using

namespace EmployeeMicroservice.Models.Entities
{
    public class EmployeeSkill
    {
        public Guid EmployeeId { get; set; }
        public Guid SkillId { get; set; }

        // Use the enum here
        public SkillLevel ProficiencyLevel { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime? AcquiredDate { get; set; }
        public bool IsPrimarySkill { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
}