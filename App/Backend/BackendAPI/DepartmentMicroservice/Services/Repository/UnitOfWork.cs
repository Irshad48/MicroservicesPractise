using DepartmentMicroservice.Data;
using DepartmentMicroservice.Interfaces;
using System.Threading.Tasks;

namespace DepartmentMicroservice.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IDepartmentRepository Departments { get; }

        public UnitOfWork(ApplicationDbContext context, IDepartmentRepository departmentRepository)
        {
            _context = context;
            Departments = departmentRepository;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;

        public void Dispose() => _context.Dispose();
    }
}