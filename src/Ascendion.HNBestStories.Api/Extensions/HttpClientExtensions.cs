using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Services;
using Ascendion.HNBestStories.Api.Settings;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Ascendion.HNBestStories.Api.Extensions;

public static class HttpClientExtensions
{
    private const string HackerNewsBaseUrl = "https://hacker-news.firebaseio.com/v0/";

    /// <summary>
    /// Registers the Hacker News HTTP client with resilience policies configured from HackerNewsSettings.
    /// </summary>
    public static IServiceCollection AddHackerNewsClient(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var settings = serviceProvider.GetRequiredService<HackerNewsSettings>();

        _ = services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client => client.BaseAddress = new Uri(HackerNewsBaseUrl))
            .AddStandardResilienceHandler(options =>
            {
                // Configure retry policy for transient failures (429, 5xx)
                options.Retry = new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = settings.MaxRetryAttempts,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(settings.RetryDelayMilliseconds)
                };

                // Configure circuit breaker to fail fast if service is experiencing issues
                options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = settings.CircuitBreakerFailureRatio,
                    SamplingDuration = TimeSpan.FromSeconds(settings.CircuitBreakerSamplingDurationSeconds),
                    MinimumThroughput = settings.CircuitBreakerMinimumThroughput
                };

                // Set a timeout for each attempt
                options.AttemptTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(settings.RequestTimeoutSeconds),
                };
            });

        return services;
    }
}