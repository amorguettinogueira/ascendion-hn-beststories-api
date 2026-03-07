using System.ComponentModel.DataAnnotations;

namespace Ascendion.HNBestStories.Api.Settings;

/// <summary>
/// Configuration settings for the Hacker News API client.
/// Supports environment variables in Docker containers.
/// </summary>
public class HackerNewsSettings
{
    public const string SectionName = "HackerNews";

    /// <summary>
    /// Cache duration for the best stories IDs list (in minutes).
    /// Environment variable: HACKERNEWS_BESTESTORIES_CACHE_DURATION_MINUTES
    /// </summary>
    [Range(1, 60, ErrorMessage = "Best stories cache duration must be between 1 and 60 minutes")]
    public int BestStoriesIdsCacheDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Cache duration for individual story details (in hours).
    /// Environment variable: HACKERNEWS_STORY_CACHE_DURATION_HOURS
    /// </summary>
    [Range(1, 24, ErrorMessage = "Story cache duration must be between 1 and 24 hours")]
    public int StoryCacheDurationHours { get; set; } = 1;

    /// <summary>
    /// Maximum number of stories that can be requested in a single API call.
    /// Environment variable: HACKERNEWS_MAX_STORIES_ALLOWED
    /// </summary>
    [Range(1, 500, ErrorMessage = "Max stories allowed must be between 1 and 500")]
    public int MaxStoriesAllowed { get; set; } = 30;

    /// <summary>
    /// Maximum number of parallel story fetch requests.
    /// Controls concurrency to avoid overwhelming the Hacker News API.
    /// Environment variable: HACKERNEWS_MAX_PARALLEL_REQUESTS
    /// </summary>
    [Range(1, 50, ErrorMessage = "Max parallel requests must be between 1 and 50")]
    public int MaxParallelRequests { get; set; } = 10;

    /// <summary>
    /// HTTP request timeout in seconds.
    /// Environment variable: HACKERNEWS_REQUEST_TIMEOUT_SECONDS
    /// </summary>
    [Range(1, 30, ErrorMessage = "Request timeout must be between 1 and 30 seconds")]
    public int RequestTimeoutSeconds { get; set; } = 2;

    /// <summary>
    /// Maximum number of retry attempts for transient failures.
    /// Environment variable: HACKERNEWS_MAX_RETRY_ATTEMPTS
    /// </summary>
    [Range(0, 10, ErrorMessage = "Max retry attempts must be between 0 and 10")]
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Initial retry delay in milliseconds.
    /// Environment variable: HACKERNEWS_RETRY_DELAY_MILLISECONDS
    /// </summary>
    [Range(1, 5000, ErrorMessage = "Retry delay must be between 1 and 5000 milliseconds")]
    public int RetryDelayMilliseconds { get; set; } = 200;

    /// <summary>
    /// Circuit breaker failure ratio (0.0 to 1.0).
    /// Environment variable: HACKERNEWS_CIRCUIT_BREAKER_FAILURE_RATIO
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "Circuit breaker failure ratio must be between 0.0 and 1.0")]
    public double CircuitBreakerFailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Circuit breaker sampling duration in seconds.
    /// Environment variable: HACKERNEWS_CIRCUIT_BREAKER_SAMPLING_DURATION_SECONDS
    /// </summary>
    [Range(1, 300, ErrorMessage = "Circuit breaker sampling duration must be between 1 and 300 seconds")]
    public int CircuitBreakerSamplingDurationSeconds { get; set; } = 30;

    /// <summary>
    /// Minimum throughput for circuit breaker (minimum number of requests to evaluate the failure ratio).
    /// Environment variable: HACKERNEWS_CIRCUIT_BREAKER_MINIMUM_THROUGHPUT
    /// </summary>
    [Range(1, 100, ErrorMessage = "Circuit breaker minimum throughput must be between 1 and 100")]
    public int CircuitBreakerMinimumThroughput { get; set; } = 5;
}
