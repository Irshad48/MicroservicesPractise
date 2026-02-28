using DepartmentMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DepartmentMicroservice.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<IEnumerable<Department>> GetAllActiveAsync();
        Task<Department?> GetByIdActiveAsync(Guid id);
        Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
        Task<Department> CreateAsync(Department department);
        Task<Department?> UpdateAsync(Guid id, Department department);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}