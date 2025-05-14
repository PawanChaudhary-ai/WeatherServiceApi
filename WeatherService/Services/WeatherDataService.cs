using System.Text.Json;
using WeatherService.API.Interfaces;
using WeatherService.API.Models;

namespace WeatherService.API.Services;

public class WeatherDataService : IWeather
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _config;
    private readonly IWeatherFileHelper _fileHelper;

    public WeatherDataService(IHttpClientFactory clientFactory, IConfiguration config, IWeatherFileHelper fileHelper)
    {
        _clientFactory = clientFactory;
        _config = config;
        _fileHelper = fileHelper;
    }

    public async Task<WeatherResponse> ProcessWeatherDataAsync(string inputFile, string cityId)
    {
        var apiKey = _config["ApiWeather:ApiKey"];
        var baseUrl = _config["ApiWeather:BaseUrl"];
        var outputDir = _config["OutputDirectory"];
        OpenWeatherMapResponse? openWeatherMapResponse = new OpenWeatherMapResponse();
        try
        {
            var cities = _fileHelper.ReadCityIds(inputFile, cityId);
            Directory.CreateDirectory(outputDir);

            foreach (var (_cityId, _cityName) in cities)
            {
                var url = $"{baseUrl}?id={_cityId}&appid={apiKey}&units=metric";
                var client = _clientFactory.CreateClient();

                var response = await client.GetAsync(url);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    openWeatherMapResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(content, options);
                    _fileHelper.WriteWeatherToFile(outputDir, _cityName, openWeatherMapResponse!);
                }
            }

            if (openWeatherMapResponse?.Id != null)
            {
                return new WeatherResponse
                {
                    Message = "Success",
                    StatusCode = "200",
                    Data = openWeatherMapResponse!
                };
            }
            else
            {
                return new WeatherResponse
                {
                    Message = "Not Data Found",
                    StatusCode = "404",
                    Data = null
                };
            }
        }
        catch (Exception ex)
        {
            return new WeatherResponse
            {
                Message = $"Error: {ex.Message}",
                StatusCode = "500",
                Data = null
            };
        }
    }
}
