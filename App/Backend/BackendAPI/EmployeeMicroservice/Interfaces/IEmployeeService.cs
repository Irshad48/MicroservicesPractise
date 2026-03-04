using EmployeeMicroservice.Models;

namespace EmployeeMicroservice.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(Guid id);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee?> UpdateAsync(Guid id, Employee employee);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(Guid departmentId);
    }
}