using EmployeeMicroservice.Models.Enums;  // Add this if you want to use the enum

namespace EmployeeMicroservice.DTOs.Responses.EmployeeSkill
{
    public class EmployeeSkillResponseDto
    {
        // Employee Info
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;

        // Skill Info
        public Guid SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillCategory { get; set; } = string.Empty;

        // Relationship Info
        public string ProficiencyLevel { get; set; } = string.Empty;  // Store as string for response
        public int YearsOfExperience { get; set; }
        public DateTime? AcquiredDate { get; set; }
        public bool IsPrimarySkill { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}