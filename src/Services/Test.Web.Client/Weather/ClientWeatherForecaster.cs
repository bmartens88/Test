using System.Net.Http.Json;

namespace Test.Web.Client.Weather;

internal sealed class ClientWeatherForecaster(HttpClient httpClient) : IWeatherForecaster
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weather-forecast") ??
               throw new IOException("no weather forecast!");
    }
}