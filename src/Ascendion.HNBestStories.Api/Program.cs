using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Endpoints;
using Ascendion.HNBestStories.Api.Extensions;
using Ascendion.HNBestStories.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration (required for .env file support in Docker)
builder.Configuration.AddEnvironmentVariables();

// Add settings from configuration with validation
builder.Services.AddApplicationSettings(builder.Configuration);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

// Register services
builder.Services.AddScoped<IBestStoriesService, BestStoriesService>();
builder.Services.AddHackerNewsClient();

// Add other services and configurations as needed
var app = builder.Build();
_ = app.MapOpenApi();
app.UseHttpsRedirection();

// Map endpoints
app.MapBestStoriesEndpoints();

app.Run();