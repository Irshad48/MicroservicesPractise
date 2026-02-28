using EmployeeMicroservice.Data;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models;
using EmployeeMicroservice.Services.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Services.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Ensure employees are returned with their skills (eager load)
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetByDepartmentIdAsync(Guid departmentId)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId && e.IsActive)
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
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
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        // Return employee with navigation properties loaded.
        // Important: Do NOT use AsNoTracking here so controller mapping updates the tracked entity.
        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
        }

        // Implement the interface method (maps to base AddAsync)
        public async Task<Employee> CreateAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            return await base.AddAsync(employee);
        }

        // Updated UpdateAsync: controller maps onto the tracked entity (from GetByIdAsync),
        // so just update timestamp and return the tracked instance.
        public async Task<Employee?> UpdateAsync(Guid id, Employee employee)
        {
            var existingEmployee = await GetByIdAsync(id);
            if (existingEmployee == null)
                return null;

            existingEmployee.UpdatedAt = DateTime.UtcNow;

            return existingEmployee;
        }

        // Keep the base UpdateAsync method for other uses
        public new async Task<Employee?> UpdateAsync(Employee employee)
        {
            return await base.UpdateAsync(employee);
        }
    }
}