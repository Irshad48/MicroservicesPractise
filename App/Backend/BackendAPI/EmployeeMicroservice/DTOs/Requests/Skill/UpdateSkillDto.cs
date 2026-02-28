using System.ComponentModel.DataAnnotations;

namespace EmployeeMicroservice.DTOs.Requests.Skill
{
    public class UpdateSkillDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Skill name must be between 2 and 100 characters")]
        public string? Name { get; set; }

        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string? Category { get; set; }
    }
}