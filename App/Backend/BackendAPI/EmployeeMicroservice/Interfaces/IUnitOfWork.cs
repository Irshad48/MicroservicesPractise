namespace EmployeeMicroservice.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        ISkillRepository Skills { get; }           
        IEmployeeSkillRepository EmployeeSkills { get; }

        Task<int> CompleteAsync();
        Task<bool> SaveChangesAsync();
    }
}