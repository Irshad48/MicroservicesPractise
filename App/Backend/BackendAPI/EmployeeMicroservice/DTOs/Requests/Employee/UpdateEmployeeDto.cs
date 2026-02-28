using System;
using System.ComponentModel.DataAnnotations;
namespace EmployeeMicroservice.DTOs.Requests.Employee
{
    public class UpdateEmployeeDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Salary { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsActive { get; set; }
    }
}
