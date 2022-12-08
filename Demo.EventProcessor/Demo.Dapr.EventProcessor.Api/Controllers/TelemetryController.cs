using Demo.EventProcessor;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Dapr.EventProcessor.Api.Controllers
{
    [ApiController]
    public class TelemetryController : Controller
    {
        private readonly ILogger<TelemetryController> _logger;

        public TelemetryController(ILogger<TelemetryController> logger)
        {
            _logger = logger;
        }

        [HttpGet("status")]
        public IActionResult Get()
        {
            _logger.LogInformation("Processing reqest - status");
            return new OkObjectResult("OK");
        }

        [HttpPost("ProcessTelemetry")]
        public async Task<ActionResult> ProcessTelemetry(TelemetryModel telemetryModel)
        {
            _logger.LogInformation("Processing reqest - processtelemetry");
            Logic.Execute(telemetryModel);
            return Ok();
        }
    }
}
