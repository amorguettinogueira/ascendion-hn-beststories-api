using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Abstractions;

public interface IWeatherForecastService
{
    WeatherForecast[] GetForecast();
}
