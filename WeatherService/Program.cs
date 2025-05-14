using WeatherService.API.Helpers;
using WeatherService.API.Interfaces;
using WeatherService.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSingleton<IWeatherFileHelper, WeatherFileHelper>();
builder.Services.AddSingleton<IWeather, WeatherDataService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
