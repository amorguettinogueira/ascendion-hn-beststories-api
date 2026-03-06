using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Services;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Ascendion.HNBestStories.Api.Extensions;

public static class HttpClientExtensions
{
    private const string HackerNewsBaseUrl = "https://hacker-news.firebaseio.com/v0/";

    /// <summary>
    /// Registers the Hacker News HTTP client with resilience policies.
    /// </summary>
    public static IServiceCollection AddHackerNewsClient(this IServiceCollection services)
    {
        _ = services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client => client.BaseAddress = new Uri(HackerNewsBaseUrl))
            .AddStandardResilienceHandler(options =>
            {
                // Configure retry policy for transient failures (429, 5xx)
                options.Retry = new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(200)
                };

                // Configure circuit breaker to fail fast if service is experiencing issues
                options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 5
                };

                // Set a timeout for each attempt
                options.AttemptTimeout = new HttpTimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(2),
                };
            });

        return services;
    }
}