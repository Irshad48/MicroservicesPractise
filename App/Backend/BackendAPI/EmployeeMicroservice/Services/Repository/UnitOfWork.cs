using EmployeeMicroservice.Data;
using EmployeeMicroservice.Interfaces;

namespace EmployeeMicroservice.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IEmployeeRepository Employees { get; private set; }
        public ISkillRepository Skills { get; private set; }              // Add this
        public IEmployeeSkillRepository EmployeeSkills { get; private set; } // Add this

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Employees = new EmployeeRepository(_context);
            Skills = new SkillRepository(_context);                    // Add this
            EmployeeSkills = new EmployeeSkillRepository(_context);    // Add this
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<bool> SaveChangesAsync()  // Add this method
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}