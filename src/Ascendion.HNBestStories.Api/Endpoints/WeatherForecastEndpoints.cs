using Ascendion.HNBestStories.Api.Abstractions;

namespace Ascendion.HNBestStories.Api.Endpoints;

public static class WeatherForecastEndpoints
{
    public static void MapWeatherForecastEndpoints(this WebApplication app) =>
        _ = app.MapGet("/weatherforecast", (IWeatherForecastService service) => service.GetForecast())
           .AddOpenApiOperationTransformer((operation, context, ct) =>
           {
               operation.Summary = "Retrieves a 5-day weather forecast";
               operation.Description = "Returns a 5-day weather forecast with daily temperature in Celsius and weather conditions.";
               return Task.CompletedTask;
           });
}