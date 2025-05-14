namespace WeatherService.API.Models
{
    public class WeatherResponse
    {
        public string? Message { get; set; }
        public string? StatusCode { get; set; }
        public OpenWeatherMapResponse? Data { get; set; }

        
    }
}
