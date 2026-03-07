using System.ComponentModel.DataAnnotations;

namespace Ascendion.HNBestStories.Api.Settings;

/// <summary>
/// Configuration settings for the Hacker News API client.
/// All settings support environment variable override via configuration binding.
/// Environment variable names should use double underscores for nested properties (e.g., HackerNews__MaxRetryAttempts).
/// </summary>
public sealed class HackerNewsSettings
{
    /// <summary>
    /// The configuration section name for Hacker News settings.
    /// Used when binding configuration values.
    /// </summary>
    public const string SectionName = "HackerNews";

    /// <summary>
    /// Gets or sets the cache duration for the best stories IDs list in minutes.
    /// </summary>
    /// <remarks>
    /// Default: 5 minutes
    /// Valid range: 1-60 minutes
    /// Lower values increase API calls to Hacker News; higher values may serve stale data.
    /// </remarks>
    [Range(1, 60, ErrorMessage = "Best stories cache duration must be between 1 and 60 minutes")]
    public int BestStoriesIdsCacheDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the cache duration for individual story details in hours.
    /// </summary>
    /// <remarks>
    /// Default: 1 hour
    /// Valid range: 1-24 hours
    /// Balances freshness of story metadata with API load.
    /// </remarks>
    [Range(1, 24, ErrorMessage = "Story cache duration must be between 1 and 24 hours")]
    public int StoryCacheDurationHours { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of stories that can be requested in a single API call.
    /// </summary>
    /// <remarks>
    /// Default: 30 stories
    /// Valid range: 1-500 stories
    /// Prevents abuse and limits resource consumption on the server.
    /// </remarks>
    [Range(1, 200, ErrorMessage = "Max stories allowed must be between 1 and 200")]
    public int MaxStoriesAllowed { get; set; } = 30;

    /// <summary>
    /// Gets or sets the maximum number of parallel story fetch requests.
    /// Controls concurrency when fetching individual story details.
    /// </summary>
    /// <remarks>
    /// Default: 10 parallel requests
    /// Valid range: 1-50 requests
    /// Higher values improve performance but may overwhelm the Hacker News API.
    /// </remarks>
    [Range(1, 50, ErrorMessage = "Max parallel requests must be between 1 and 50")]
    public int MaxParallelRequests { get; set; } = 10;

    /// <summary>
    /// Gets or sets the HTTP request timeout in seconds.
    /// Applies to individual HTTP requests to the Hacker News API.
    /// </summary>
    /// <remarks>
    /// Default: 2 seconds
    /// Valid range: 1-30 seconds
    /// Prevents hanging requests; consider network latency when setting.
    /// </remarks>
    [Range(1, 30, ErrorMessage = "Request timeout must be between 1 and 30 seconds")]
    public int RequestTimeoutSeconds { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for transient failures.
    /// Applies to the resilience retry policy.
    /// </summary>
    /// <remarks>
    /// Default: 3 attempts
    /// Valid range: 0-10 attempts
    /// 0 means no retries; higher values improve resilience but increase latency on failures.
    /// </remarks>
    [Range(0, 10, ErrorMessage = "Max retry attempts must be between 0 and 10")]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial retry delay in milliseconds.
    /// Uses exponential backoff, so actual delays are multiplied by 2^attempt.
    /// </summary>
    /// <remarks>
    /// Default: 200 milliseconds
    /// Valid range: 1-5000 milliseconds
    /// Higher values space out retries more; useful for heavily loaded services.
    /// </remarks>
    [Range(1, 5000, ErrorMessage = "Retry delay must be between 1 and 5000 milliseconds")]
    public int RetryDelayMilliseconds { get; set; } = 200;

    /// <summary>
    /// Gets or sets the circuit breaker failure ratio threshold.
    /// Opens the circuit when the failure rate exceeds this ratio.
    /// </summary>
    /// <remarks>
    /// Default: 0.5 (50%)
    /// Valid range: 0.0-1.0
    /// 0.5 means circuit opens if 50% or more requests fail within the sampling duration.
    /// </remarks>
    [Range(0.0, 1.0, ErrorMessage = "Circuit breaker failure ratio must be between 0.0 and 1.0")]
    public double CircuitBreakerFailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the circuit breaker sampling duration in seconds.
    /// The time window over which failure ratio is calculated.
    /// </summary>
    /// <remarks>
    /// Default: 30 seconds
    /// Valid range: 1-300 seconds
    /// Defines the rolling window for failure rate evaluation.
    /// </remarks>
    [Range(1, 300, ErrorMessage = "Circuit breaker sampling duration must be between 1 and 300 seconds")]
    public int CircuitBreakerSamplingDurationSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the minimum throughput required for circuit breaker evaluation.
    /// The minimum number of requests in the sampling duration to trigger failure ratio evaluation.
    /// </summary>
    /// <remarks>
    /// Default: 5 requests
    /// Valid range: 1-100 requests
    /// Prevents circuit breaker from opening based on very few requests.
    /// </remarks>
    [Range(1, 100, ErrorMessage = "Circuit breaker minimum throughput must be between 1 and 100")]
    public int CircuitBreakerMinimumThroughput { get; set; } = 5;
}