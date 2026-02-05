using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ILogger<BaseApiController> _logger;

        protected BaseApiController(ILogger<BaseApiController> logger)
        {
            _logger = logger;
        }

        protected IActionResult HandleError(Exception ex, string operation)
        {
            _logger.LogError(ex, "Error during {Operation}", operation);

            return StatusCode(500, new
            {
                Message = $"An error occurred while {operation}",
                Error = ex.Message
            });
        }

        protected IActionResult NotFoundResponse(string resourceName, object id)
        {
            _logger.LogWarning("{ResourceName} with ID {Id} not found", resourceName, id);
            return NotFound(new { Message = $"{resourceName} with ID {id} not found" });
        }

        protected IActionResult BadRequestResponse(string message)
        {
            _logger.LogWarning("Bad request: {Message}", message);
            return BadRequest(new { Message = message });
        }
    }
}