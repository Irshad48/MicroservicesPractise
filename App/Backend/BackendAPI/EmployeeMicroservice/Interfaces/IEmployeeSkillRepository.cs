using EmployeeMicroservice.Models.Entities;
using EmployeeMicroservice.Models.Enums;

namespace EmployeeMicroservice.Interfaces
{
    public interface IEmployeeSkillRepository : IRepository<EmployeeSkill>
    {
        Task<EmployeeSkill?> GetEmployeeSkillAsync(Guid employeeId, Guid skillId);
        Task<IEnumerable<EmployeeSkill>> GetEmployeeSkillsWithDetailsAsync(Guid employeeId);
        Task<IEnumerable<EmployeeSkill>> GetSkillEmployeesWithDetailsAsync(Guid skillId);
        Task<bool> ExistsAsync(Guid employeeId, Guid skillId);
        Task<IEnumerable<EmployeeSkill>> GetEmployeesBySkillAndLevelAsync(Guid skillId, SkillLevel? minLevel = null);
        Task SetPrimarySkillAsync(Guid employeeId, Guid skillId);
        Task<bool> DeleteEmployeeSkillAsync(Guid employeeId, Guid skillId);
    }
}