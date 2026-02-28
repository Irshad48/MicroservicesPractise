using EmployeeMicroservice.Data;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models.Entities;
using EmployeeMicroservice.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace EmployeeMicroservice.Services.Repository
{
    public class EmployeeSkillRepository : Repository<EmployeeSkill>, IEmployeeSkillRepository
    {
        public EmployeeSkillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<EmployeeSkill?> GetEmployeeSkillAsync(Guid employeeId, Guid skillId)
        {
            return await _context.EmployeeSkills
                .Include(es => es.Employee)
                .Include(es => es.Skill)
                .FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId);
        }

        public async Task<IEnumerable<EmployeeSkill>> GetEmployeeSkillsWithDetailsAsync(Guid employeeId)
        {
            return await _context.EmployeeSkills
                .Include(es => es.Skill)
                .Where(es => es.EmployeeId == employeeId)
                .OrderByDescending(es => es.IsPrimarySkill)
                .ThenByDescending(es => es.ProficiencyLevel)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeSkill>> GetSkillEmployeesWithDetailsAsync(Guid skillId)
        {
            return await _context.EmployeeSkills
                .Include(es => es.Employee)
                .Where(es => es.SkillId == skillId)
                .OrderByDescending(es => es.ProficiencyLevel)
                .ThenByDescending(es => es.YearsOfExperience)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid employeeId, Guid skillId)
        {
            return await _context.EmployeeSkills
                .AnyAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId);
        }

        public async Task<IEnumerable<EmployeeSkill>> GetEmployeesBySkillAndLevelAsync(Guid skillId, SkillLevel? minLevel = null)
        {
            var query = _context.EmployeeSkills
                .Include(es => es.Employee)
                .Where(es => es.SkillId == skillId);

            if (minLevel.HasValue)
            {
                query = query.Where(es => es.ProficiencyLevel >= minLevel.Value);
            }

            return await query
                .OrderByDescending(es => es.ProficiencyLevel)
                .ThenByDescending(es => es.YearsOfExperience)
                .ToListAsync();
        }

        public async Task SetPrimarySkillAsync(Guid employeeId, Guid skillId)
        {
            // Get all skills for this employee
            var employeeSkills = await _context.EmployeeSkills
                .Where(es => es.EmployeeId == employeeId)
                .ToListAsync();

            // Set all to false
            foreach (var es in employeeSkills)
            {
                es.IsPrimarySkill = false;
            }

            // Set the selected one as primary
            var primarySkill = employeeSkills.FirstOrDefault(es => es.SkillId == skillId);
            if (primarySkill != null)
            {
                primarySkill.IsPrimarySkill = true;
                primarySkill.UpdatedAt = DateTime.UtcNow;
            }
        }
        public async Task<bool> DeleteEmployeeSkillAsync(Guid employeeId, Guid skillId)
        {
            var employeeSkill = await _context.EmployeeSkills
                .FirstOrDefaultAsync(es => es.EmployeeId == employeeId && es.SkillId == skillId);

            if (employeeSkill == null)
                return false;

            _context.EmployeeSkills.Remove(employeeSkill);
            return true;
        }
    }
}
