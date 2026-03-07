using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Endpoints;
using Ascendion.HNBestStories.Api.Extensions;
using Ascendion.HNBestStories.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
// This enables .env file support in Docker containers and local development
builder.Configuration.AddEnvironmentVariables();

// Configure application settings with validation
builder.Services.AddApplicationSettings(builder.Configuration);

// Add core services to the container
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

// Register business logic services
builder.Services.AddScoped<IBestStoriesService, BestStoriesService>();
builder.Services.AddScoped<IStoriesRequestValidator, StoriesRequestValidator>();

// Register HTTP client with resilience policies
builder.Services.AddHackerNewsClient();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map API endpoints
app.MapBestStoriesEndpoints();

app.Run();