using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;

namespace Chat.Plugins
{
    public class GetWeather
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GetWeather(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        [KernelFunction("get_weather_forecast")]
        [Description("get the 7 day weather forecast for a give latitude and longitude")]
        [return: Description("returns the 7 day forecast in 12 hour increments, formated as Digital Weather Markup Language (DWML)")]
        public async Task<string> GetWeatherPointAsync(decimal latitude, decimal longitude)
        {
            var forecastUrl = await GetForecastURL(latitude, longitude)
                ?? throw new InvalidOperationException("Forecast URL not found in response.");

            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, forecastUrl);
            request.Headers.Add("User-Agent", "myapplication-udemy-course");
            request.Headers.Add("accept", "application/vnd.noaa.dwml+xml");

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string?> GetForecastURL(decimal latitude, decimal longitude)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.weather.gov/points/{latitude},{longitude}";
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "myapplication-udemy-course");

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (doc.RootElement.TryGetProperty("properties", out var properties) &&
                properties.TryGetProperty("forecast", out var forecast))
            {
                return forecast.GetString();
            }
            return null;
        }
    }
}