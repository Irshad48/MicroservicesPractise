using EmployeeMicroservice.Data;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Services.Repository;
using System;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IEmployeeRepository _employeeRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEmployeeRepository Employees =>
            _employeeRepository ??= new EmployeeRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                var changes = await _context.SaveChangesAsync();
                return changes > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}