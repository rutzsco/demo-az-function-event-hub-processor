using Microsoft.AspNetCore.Mvc;

namespace Demo.Dapr.EventProcessor.Api.Controllers
{
    [ApiController]
    public class StatusController : Controller
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet("status")]
        public IActionResult Get()
        {
            _logger.LogInformation("Processing reqest - status");
            return new OkObjectResult("OK");
        }
    }
}
