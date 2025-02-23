using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    [HttpGet("", Name = "GetWeatherForecast")]
	public IEnumerable<WeatherForecast> GetFiveDayForecast()
	{
		var forecast = Enumerable.Range(1, 5).Select(index =>
			new WeatherForecast
			(
			 DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			 Random.Shared.Next(-20, 55),
			 _summaries[Random.Shared.Next(_summaries.Length)]
			))
			.ToArray();
		return forecast;
	}

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    private readonly string[] _summaries =
	[
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
}

