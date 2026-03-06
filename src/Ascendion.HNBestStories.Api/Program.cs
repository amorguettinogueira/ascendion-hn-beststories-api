using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Endpoints;
using Ascendion.HNBestStories.Api.Extensions;
using Ascendion.HNBestStories.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

// Register services
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<IBestStoriesService, BestStoriesService>();
builder.Services.AddHackerNewsClient();

// Add other services and configurations as needed
var app = builder.Build();
_ = app.MapOpenApi();
app.UseHttpsRedirection();

// Map endpoints
app.MapWeatherForecastEndpoints();
app.MapBestStoriesEndpoints();

app.Run();