namespace FancyMCP;

public class WeatherForecast(DateOnly date, int temperatureC, string? summary)
{
    public DateOnly Date { get; init; } = date;
    public int TemperatureC { get; init; } = temperatureC;
    public string? Summary { get; init; } = summary;

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
    