using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using WeatherService.API.Interfaces;
using WeatherService.API.Models;
using WeatherService.API.Services;

namespace WeatherService.Tests
{
    [TestFixture]
    public class WeatherDataServiceTests
    {
        private Mock<IConfiguration> _mockConfig;
        private Mock<IHttpClientFactory> _mockFactory;
        private Mock<IWeatherFileHelper> _mockFileHelper;
        private WeatherDataService _weatherService;

        [SetUp]
        public void Setup()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockFactory = new Mock<IHttpClientFactory>();
            _mockFileHelper = new Mock<IWeatherFileHelper>();

            _weatherService = new WeatherDataService(
                _mockFactory.Object,
                _mockConfig.Object,
                _mockFileHelper.Object
            );
        }

        [Test]
        public async Task ProcessWeatherDataAsync_SuccessfulResponse_ReturnsWeatherData()
        {
            // Arrange
            int cityId = 2643741;
            var cityName = "TestCity";
            var inputFile = "city_ids.txt";
            var outputDir = "output";

            var expectedWeather = new OpenWeatherMapResponse 
            { 
                Id = cityId,
                Name = cityName,
                Main = new Main { Temp = 25.5, Humidity = 70 }
            };
            var jsonContent = JsonSerializer.Serialize(expectedWeather);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse));

            _mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _mockFileHelper.Setup(f => f.ReadCityIds(inputFile, Convert.ToString(cityId)))
                           .Returns(new List<(string, string)> { (Convert.ToString(cityId), cityName) });

            _mockFileHelper.Setup(f => f.WriteWeatherToFile(outputDir, cityName, It.IsAny<OpenWeatherMapResponse>()));

            _mockConfig.Setup(c => c["ApiWeather:ApiKey"]).Returns("dummy_api_key");
            _mockConfig.Setup(c => c["ApiWeather:BaseUrl"]).Returns("https://api.test.com");
            _mockConfig.Setup(c => c["OutputDirectory"]).Returns(outputDir);

            // Act
            var result = await _weatherService.ProcessWeatherDataAsync(inputFile, Convert.ToString(cityId));

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo("200"));
            Assert.That(result.Message, Is.EqualTo("Success"));
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.Id, Is.EqualTo(cityId));
        }

        [Test]
        public async Task ProcessWeatherDataAsync_EmptyResponse_ReturnsNotFoundResponse()
        {
            // Arrange
            var cityId = "123";
            var cityName = "TestCity";
            var inputFile = "city_ids.txt";

            var expectedWeather = new OpenWeatherMapResponse();
            var jsonContent = JsonSerializer.Serialize(expectedWeather);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse));

            _mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _mockFileHelper.Setup(f => f.ReadCityIds(inputFile, cityId))
                           .Returns(new List<(string, string)> { (cityId, cityName) });

            _mockConfig.Setup(c => c["ApiWeather:ApiKey"]).Returns("dummy_api_key");
            _mockConfig.Setup(c => c["ApiWeather:BaseUrl"]).Returns("https://api.test.com");
            _mockConfig.Setup(c => c["OutputDirectory"]).Returns("output");

            // Act
            var result = await _weatherService.ProcessWeatherDataAsync(inputFile, cityId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo("404"));
            Assert.That(result.Message, Is.EqualTo("Not Data Found"));
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task ProcessWeatherDataAsync_HttpRequestFailed_ReturnsErrorResponse()
        {
            // Arrange
            var cityId = "123";
            var cityName = "TestCity";
            var inputFile = "city_ids.txt";

            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse));

            _mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _mockFileHelper.Setup(f => f.ReadCityIds(inputFile, cityId))
                           .Returns(new List<(string, string)> { (cityId, cityName) });

            _mockConfig.Setup(c => c["ApiWeather:ApiKey"]).Returns("dummy_api_key");
            _mockConfig.Setup(c => c["ApiWeather:BaseUrl"]).Returns("https://api.test.com");
            _mockConfig.Setup(c => c["OutputDirectory"]).Returns("output");

            // Act
            var result = await _weatherService.ProcessWeatherDataAsync(inputFile, cityId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo("404"));
            Assert.That(result.Message, Contains.Substring("Not Data Found"));
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task ProcessWeatherDataAsync_ExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var cityId = "123";
            var inputFile = "city_ids.txt";

            _mockFileHelper.Setup(f => f.ReadCityIds(inputFile, cityId))
                           .Throws(new Exception("Test exception"));

            _mockConfig.Setup(c => c["ApiWeather:ApiKey"]).Returns("dummy_api_key");
            _mockConfig.Setup(c => c["ApiWeather:BaseUrl"]).Returns("https://api.test.com");
            _mockConfig.Setup(c => c["OutputDirectory"]).Returns("output");

            // Act
            var result = await _weatherService.ProcessWeatherDataAsync(inputFile, cityId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo("500"));
            Assert.That(result.Message, Contains.Substring("Error: Test exception"));
            Assert.IsNull(result.Data);
        }

        // Helper for mocking HttpClient
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }
    }
}
