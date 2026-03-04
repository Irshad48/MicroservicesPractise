namespace EmployeeMicroservice.Services.External
{
    public interface IDepartmentServiceClient
    {
        Task<bool> DepartmentExistsAsync(Guid departmentId);
    }
}