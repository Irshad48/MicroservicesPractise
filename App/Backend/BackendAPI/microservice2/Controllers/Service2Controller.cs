using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace microservice1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Service2Controller : ControllerBase
    {

        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            string message = "Hello from Microservice 2!";
            return Ok(message);
        }


        [HttpGet("health")]
        public IActionResult health()
        {
            return Ok("Microservice 2 working fine");
        }
    }
}
