using WeatherService.API.Models;

namespace WeatherService.API.Interfaces
{
    public interface IWeatherFileHelper
    {
        public List<(string Id, string Name)> ReadCityIds(string fileName, string cityId);
        public void WriteWeatherToFile(string outputDir, string cityName, OpenWeatherMapResponse weather);
    }
}
