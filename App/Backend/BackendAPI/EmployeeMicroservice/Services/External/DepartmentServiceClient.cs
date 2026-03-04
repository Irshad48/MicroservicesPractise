using System.Net;
using EmployeeMicroservice.Services.External;

namespace EmployeeMicroservice.Services.External
{
    public class DepartmentServiceClient : IDepartmentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DepartmentServiceClient> _logger;

        public DepartmentServiceClient(HttpClient httpClient,
                                       ILogger<DepartmentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> DepartmentExistsAsync(Guid departmentId)
        {
            try
            {
                var response = await _httpClient
                    .GetAsync($"/api/department/{departmentId}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return false;

                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error validating DepartmentId {DepartmentId}", departmentId);

                throw;
            }
        }
    }
}