using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace microservice1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Service1Controller : ControllerBase
    {
        // Using IHttpClientFactory for better HttpClient management - register in Program.cs
        private readonly IHttpClientFactory _httpClientFactory;
        public Service1Controller(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("message")]
        public async Task<IActionResult> GetMessage()
        {
            // Simplest way to call another microservice
            // In production, consider using HttpClientFactory or other robust methods
            // this is not recommended for production code due to potential socket exhaustion issues

            /*var httpClient = new HttpClient();
            string messageFromService2 = httpClient.GetStringAsync("https://localhost:7132/api/service2/message").Result;
            string message = $"Hello from Microservice 1! \n{messageFromService2}";*/

            // Improved version with error handling
            // using async/await - remember to add 'async' to method signature - also change return type to Task<IActionResult> because async returns a Task     

            /*var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync("https://localhost:7132/api/service2/message");
            if(!httpResponse.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calling Microservice 2");
            }
            string messageFromService2 = await httpResponse.Content.ReadAsStringAsync();
            string message = $"Hello from Microservice 1! \n{messageFromService2}";*/

            // Best practice - using HttpClientFactory
            //create HttpClient from factory
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://localhost:7132/api/service2/message");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error calling Microservice 2");
            }

            string messageFromService2 = await response.Content.ReadAsStringAsync();
            string message = $"Hello from Microservice 1! \n{messageFromService2}";
            return Ok(message);
        }


        [HttpGet("health")]
        public IActionResult health()
        {
            return Ok("Microservice 1 working fine");
        }
    }
}
