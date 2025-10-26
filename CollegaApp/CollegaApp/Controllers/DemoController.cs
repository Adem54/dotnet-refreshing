using CollegaApp.MyLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace CollegaApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly ILogger<DemoController> _logger;
        public DemoController(ILogger<DemoController> myLogger)
        {
            _logger = myLogger;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint was called.");
            return Ok("DemoController is working!");
        }


        [HttpGet("index")]
        public IActionResult Index() 
        {
            _logger.LogTrace("Log mesage from trace method");
            _logger.LogDebug("Log mesage from Debug method");
            _logger.LogInformation("Log mesage from Information method");
            _logger.LogWarning("Log mesage from warning method");
            _logger.LogError("Log mesage from error method");
            _logger.LogCritical("Log mesage from critical method");
            return Ok();
        }
    }
}
