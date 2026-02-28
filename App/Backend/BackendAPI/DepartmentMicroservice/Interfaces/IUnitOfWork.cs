using System.Threading.Tasks;

namespace DepartmentMicroservice.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDepartmentRepository Departments { get; }
        Task<int> CompleteAsync();
        Task<bool> SaveChangesAsync();
    }
}