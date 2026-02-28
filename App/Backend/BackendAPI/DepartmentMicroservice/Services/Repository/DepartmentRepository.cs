using DepartmentMicroservice.Data;
using DepartmentMicroservice.Interfaces;
using DepartmentMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DepartmentMicroservice.Services.Repository
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Department>> GetAllActiveAsync()
        {
            return await _context.Departments
                .Where(d => d.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department?> GetByIdActiveAsync(Guid id)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
        }

        public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
        {
            var q = _context.Departments.Where(d => d.IsActive && d.Name.ToLower() == name.Trim().ToLower());
            if (excludeId.HasValue) q = q.Where(d => d.Id != excludeId.Value);
            return await q.AnyAsync();
        }

        public async Task<Department> CreateAsync(Department department)
        {
            department.CreatedAt = DateTime.UtcNow;
            await _context.Departments.AddAsync(department);
            return department;
        }

        public async Task<Department?> UpdateAsync(Guid id, Department department)
        {
            var existing = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
            if (existing == null) return null;

            existing.Name = department.Name;
            existing.Description = department.Description;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.Departments.Update(existing);
            return existing;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var existing = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && d.IsActive);
            if (existing == null) return false;

            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.Departments.Update(existing);
            return true;
        }
    }
}