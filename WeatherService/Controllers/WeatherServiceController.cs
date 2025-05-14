using Microsoft.AspNetCore.Mvc;
using WeatherService.API.Interfaces;
using WeatherService.API.Models;
namespace WeatherService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherServiceController : ControllerBase
    {
        private readonly IWeather _weatherService;
        public WeatherServiceController(IWeather weatherService)
        {
            _weatherService = weatherService;
        }
        [HttpGet("{cityId:int}")]
        public async Task<IActionResult> Weather(string cityId)
        {
            try
            {
                WeatherResponse result = await _weatherService.ProcessWeatherDataAsync("city_ids.txt", cityId);

                if (result.StatusCode == "200")
                    return Ok(result);
                else if (result.StatusCode == "404")
                    return StatusCode(404, result);
                else
                    return StatusCode(500, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
