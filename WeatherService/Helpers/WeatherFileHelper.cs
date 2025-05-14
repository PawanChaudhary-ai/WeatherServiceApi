using System.Text.Json;
using WeatherService.API.Interfaces;
using WeatherService.API.Models;

namespace WeatherService.API.Helpers;

public class WeatherFileHelper : IWeatherFileHelper
{
    public List<(string Id, string Name)> ReadCityIds(string fileName, string cityId)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string path = Path.Combine(baseDir, "CityIDs", fileName);

        List<(string Id, string Name)> result = File.ReadAllLines(path)
            .Select(line =>
            {
                var parts = line.Split('=');
                return (parts[0], parts[1]);
            }).Where(id => id.Item1 == cityId)
            .ToList();
        return result;
    }

    public void WriteWeatherToFile(string outputDir, string cityName, OpenWeatherMapResponse weather)
    {
        var filePath = Path.Combine(outputDir, $"{cityName}_{DateTime.UtcNow:yyyyMMdd}.json");
        var json = JsonSerializer.Serialize(weather, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}
