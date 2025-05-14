using WeatherService.API.Helpers;

namespace WeatherService.Tests.Helpers
{
    [TestFixture]
    public class WeatherFileHelperTests
    {
        private WeatherFileHelper _helper;
        private string _cityIdsFolder;

        [SetUp]
        public void Setup()
        {
            _helper = new WeatherFileHelper();
            _cityIdsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CityIDs");
            Directory.CreateDirectory(_cityIdsFolder);
        }

        [Test]
        public void ReadCityIds_ReturnsCorrectCity_WhenCityIdExists()
        {
            // Arrange
            string fileName = "test_city_ids.txt";
            string cityId = "12345";
            string filePath = Path.Combine(_cityIdsFolder, fileName);

            File.WriteAllLines(filePath, new[]
            {
                "12345=London",
                "67890=Paris"
            });

            // Act
            var result = _helper.ReadCityIds(fileName, cityId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("12345"));
            Assert.That(result[0].Name, Is.EqualTo("London"));

            // Cleanup
            File.Delete(filePath);
        }

        [Test]
        public void ReadCityIds_ReturnsEmptyList_WhenCityIdNotFound()
        {
            // Arrange
            string fileName = "test_city_ids.txt";
            string cityId = "99999";
            string filePath = Path.Combine(_cityIdsFolder, fileName);

            File.WriteAllLines(filePath, new[]
            {
                "12345=London",
                "67890=Paris"
            });

            // Act
            var result = _helper.ReadCityIds(fileName, cityId);

            // Assert
            Assert.That(result, Is.Empty);

            // Cleanup
            File.Delete(filePath);
        }
    }
}
