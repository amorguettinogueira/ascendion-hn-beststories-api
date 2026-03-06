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
}
