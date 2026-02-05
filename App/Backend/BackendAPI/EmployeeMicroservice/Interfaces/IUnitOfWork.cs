using System;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        Task<int> CompleteAsync();
        Task<bool> SaveChangesAsync();
    }
}