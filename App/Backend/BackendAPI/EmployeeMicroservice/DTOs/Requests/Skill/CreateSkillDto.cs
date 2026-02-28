using System.ComponentModel.DataAnnotations;

namespace EmployeeMicroservice.DTOs.Requests.Skill
{
    public class CreateSkillDto
    {
        [Required(ErrorMessage = "Skill name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Skill name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string? Category { get; set; }
    }
}