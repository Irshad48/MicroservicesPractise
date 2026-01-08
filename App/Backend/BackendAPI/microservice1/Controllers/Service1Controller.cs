using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace microservice1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Service1Controller : ControllerBase
    {

        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            // Simplest way to call another microservice
            // In production, consider using HttpClientFactory or other robust methods
            // this is not recommended for production code due to potential socket exhaustion issues
            var httpClient = new HttpClient();
            string messageFromService2 = httpClient.GetStringAsync("https://localhost:7132/api/service2/message").Result;
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
