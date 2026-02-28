using System.ComponentModel.DataAnnotations;
using EmployeeMicroservice.Models.Enums;  // Add this using

namespace EmployeeMicroservice.DTOs.Requests.EmployeeSkill
{
    public class AssignSkillDto
    {
        [Required(ErrorMessage = "Employee ID is required")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "Skill ID is required")]
        public Guid SkillId { get; set; }

        [Required(ErrorMessage = "Proficiency level is required")]
        [Range(1, 4, ErrorMessage = "Proficiency level must be between 1 and 4")]
        public SkillLevel ProficiencyLevel { get; set; }  // Now this works

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int YearsOfExperience { get; set; }

        public DateTime? AcquiredDate { get; set; }

        public bool IsPrimarySkill { get; set; }
    }
}