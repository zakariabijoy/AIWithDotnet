using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace FancyMCP;

[McpServerToolType]
public class WeatherTools
{
    [McpServerTool, Description("Gets the weather forecast for a specific date (format: yyyy-MM-dd).")]
    public static async Task<string> GetWeatherByDate(WeatherService weatherService, string date)
    {
        var result = await weatherService.GetWeatherAsync(date);
        return result is not null ? JsonSerializer.Serialize(result) : "Weather data not found.";
    }

    [McpServerTool, Description("Gets the list of all weather forecasts.")]
    public static async Task<string> GetWeatherList(WeatherService weatherService)
    {
        var result = await weatherService.GetWeathersAsync();
        return JsonSerializer.Serialize(result);      
    }
}
