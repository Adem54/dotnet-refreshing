using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EFDBFirst.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors(PolicyName="OnlyGoogleApplications")]
    //SAdece bu controller bu cors-policy yi takip eder digerleri ise Program.cs de app.UseCors(); seklinde ise default policy yi takip eder ama burda paramtre icerisinde named policy lerden biri yazilmi ise app.UseCors("AllowAll"); gibi ise o zamn AlloAll ismindki policy yi takp eder gerisi
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
