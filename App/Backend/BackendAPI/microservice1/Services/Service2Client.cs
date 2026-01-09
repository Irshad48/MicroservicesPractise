using microservice1.Services.Interfaces;

namespace microservice1.Services
{
    public class Service2Client : IService2Client
    {
        private readonly HttpClient _httpClient;
        public Service2Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> GetMessageFromService2Async()
        {
           var response = await _httpClient.GetAsync("api/service2/message");
           response.EnsureSuccessStatusCode();
           return await response.Content.ReadAsStringAsync();
        }
    }
}
