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
            string message = "Hello from Microservice 1!";
            return Ok(message);
        }


        [HttpGet("health")]
        public IActionResult health()
        {
            return Ok("Microservice 1 working fine");
        }
    }
}
