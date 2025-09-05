
using System.Net.Http.Json;

namespace FancyMCP;

public class WeatherService
{
    readonly HttpClient httpClient;
    List<WeatherForecast> weatherForecasts = [];

    public WeatherService()
    {
        this.httpClient = new HttpClient();
    }

    public async Task<List<WeatherForecast>> GetWeathersAsync()
    {
        if (weatherForecasts.Count > 0)
        {
            return weatherForecasts;
        }

        var response = await httpClient.GetAsync($"http://localhost:5153/weatherforecast");
        if (response.IsSuccessStatusCode)
        {
            var weatherData = await response.Content.ReadFromJsonAsync<List<WeatherForecast>>();
            if (weatherData != null)
            {
                weatherForecasts = weatherData;
                return weatherForecasts;
            }
        }
        return [];
    }

    public async Task<WeatherForecast?> GetWeatherAsync(string date)
    {
        var weatherData = await GetWeathersAsync();
        return  weatherData?.FirstOrDefault(weatherData => weatherData.Date == DateOnly.Parse(date));
    }
}
