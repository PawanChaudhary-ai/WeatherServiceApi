using WeatherService.API.Models;

namespace WeatherService.API.Interfaces
{
    public interface IWeather
    {
        Task<WeatherResponse> ProcessWeatherDataAsync(string inputFile, string cityId);
    }
}
