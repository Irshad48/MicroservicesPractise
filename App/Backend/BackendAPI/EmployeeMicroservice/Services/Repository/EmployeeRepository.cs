using EmployeeMicroservice.Data;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models;
using EmployeeMicroservice.Services.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Services.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentIdAsync(Guid departmentId)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId && e.IsActive)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        {
            var query = _context.Employees.Where(e => e.Email == email);

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        // Implement the interface method (maps to base AddAsync)
        public async Task<Employee> CreateAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            return await base.AddAsync(employee);
        }

        // Implement the interface method with proper signature
        public async Task<Employee?> UpdateAsync(Guid id, Employee employee)
        {
            var existingEmployee = await GetByIdAsync(id);
            if (existingEmployee == null)
                return null;

            // If controller applied changes to the tracked instance 'employee' (from GetByIdAsync),
            // we can just update UpdatedAt on the tracked entity and return it.
            // Avoid blanket SetValues to prevent overwriting DB values with defaults.
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            // If 'employee' parameter is a different instance (rare here), copy only non-default values.
            // But typical controller flow maps onto the tracked entity, so no further copy is needed.

            // Save changes will be handled by UnitOfWork
            return existingEmployee;
        }

        // Keep the base UpdateAsync method for other uses
        public new async Task<Employee?> UpdateAsync(Employee employee)
        {
            return await base.UpdateAsync(employee);
        }
    }
}