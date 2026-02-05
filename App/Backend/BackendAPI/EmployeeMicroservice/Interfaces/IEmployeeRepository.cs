using EmployeeMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(Guid departmentId);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee?> UpdateAsync(Guid id, Employee employee);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
        Task<Employee?> GetByEmailAsync(string email);
    }
}