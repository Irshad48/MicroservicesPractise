using EmployeeMicroservice.Models;
using EmployeeMicroservice.Models.Entities;

namespace EmployeeMicroservice.Interfaces
{
    public interface ISkillRepository : IRepository<Skill>
    {
        Task<Skill?> GetSkillWithEmployeesAsync(Guid skillId);
        Task<IEnumerable<Skill>> GetSkillsByCategoryAsync(string category);
        Task<IEnumerable<string>> GetAllCategoriesAsync();
        Task<bool> IsSkillNameUniqueAsync(string name, Guid? excludeId = null);
    }
}