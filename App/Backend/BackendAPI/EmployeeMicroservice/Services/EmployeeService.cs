using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models;
using EmployeeMicroservice.Services.External;

namespace EmployeeMicroservice.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentServiceClient _departmentClient;

        public EmployeeService(
            IUnitOfWork unitOfWork,
            IDepartmentServiceClient departmentClient)
        {
            _unitOfWork = unitOfWork;
            _departmentClient = departmentClient;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _unitOfWork.Employees.GetAllAsync();
        }

        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Employees.GetByIdAsync(id);
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            // 🔹 Validate Department
            var departmentExists =
                await _departmentClient.DepartmentExistsAsync(employee.DepartmentId);

            if (!departmentExists)
                throw new ArgumentException("Invalid DepartmentId");

            // 🔹 Email uniqueness
            var emailExists =
                await _unitOfWork.Employees.EmailExistsAsync(employee.Email);

            if (emailExists)
                throw new ArgumentException("Email already exists");

            var createdEmployee =
                await _unitOfWork.Employees.CreateAsync(employee);

            await _unitOfWork.SaveChangesAsync();

            return createdEmployee;
        }

        public async Task<Employee?> UpdateAsync(Guid id, Employee employee)
        {
            var existing =
                await _unitOfWork.Employees.GetByIdAsync(id);

            if (existing == null)
                return null;

            var departmentExists =
                await _departmentClient.DepartmentExistsAsync(employee.DepartmentId);

            if (!departmentExists)
                throw new ArgumentException("Invalid DepartmentId");

            var emailExists =
                await _unitOfWork.Employees.EmailExistsAsync(employee.Email, id);

            if (emailExists)
                throw new ArgumentException("Email already exists");

            // Map updated fields
            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.DepartmentId = employee.DepartmentId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var employee =
                await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return false;

            employee.IsActive = false;
            employee.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<Employee>> GetByDepartmentIdAsync(Guid departmentId)
        {
            return await _unitOfWork.Employees.GetByDepartmentIdAsync(departmentId);
        }
    }
}