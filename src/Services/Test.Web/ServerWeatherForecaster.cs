﻿using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Test.Web.Client.Weather;

namespace Test.Web;

internal sealed class ServerWeatherForecaster(HttpClient httpClient, IHttpContextAccessor httpcontextAccessor)
    : IWeatherForecaster
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        var httpContext = httpcontextAccessor.HttpContext ??
                          throw new InvalidOperationException(
                              "No HttpContext available from the IHttpContextAccessor.");
        var accessToken = await httpContext.GetTokenAsync("access_token") ??
                          throw new InvalidOperationException("No access_token was saved");
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/weather-forecast");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WeatherForecast[]>() ??
               throw new IOException("No weather forecast!");
    }
}