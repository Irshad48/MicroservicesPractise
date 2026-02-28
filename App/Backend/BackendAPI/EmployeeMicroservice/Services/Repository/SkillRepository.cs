using EmployeeMicroservice.Data;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Models;
using EmployeeMicroservice.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace EmployeeMicroservice.Services.Repository
{
    public class SkillRepository : Repository<Skill>, ISkillRepository
    {
        public SkillRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Skill?> GetSkillWithEmployeesAsync(Guid skillId)
        {
            return await _context.Skills
                .Include(s => s.EmployeeSkills)
                    .ThenInclude(es => es.Employee)
                .FirstOrDefaultAsync(s => s.Id == skillId);
        }

        public async Task<IEnumerable<Skill>> GetSkillsByCategoryAsync(string category)
        {
            return await _context.Skills
                .Where(s => s.Category != null && s.Category.ToLower() == category.ToLower())
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            return await _context.Skills
                .Where(s => s.Category != null)
                .Select(s => s.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<bool> IsSkillNameUniqueAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Skills.Where(s => s.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}