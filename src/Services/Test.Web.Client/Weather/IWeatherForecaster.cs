namespace Test.Web.Client.Weather;

public interface IWeatherForecaster
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
}