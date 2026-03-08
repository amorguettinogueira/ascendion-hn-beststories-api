using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Exceptions;
using Ascendion.HNBestStories.Api.Models;
using Ascendion.HNBestStories.Api.Settings;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Ascendion.HNBestStories.Api.Services;

/// <inheritdoc />
public sealed class HackerNewsClient(
    HttpClient httpClient,
    IMemoryCache cache,
    HackerNewsSettings settings,
    ILogger<HackerNewsClient> logger) : IHackerNewsClient
{
    private readonly HttpClient _httpClient = httpClient
        ?? throw new ArgumentNullException(nameof(httpClient));

    private readonly IMemoryCache _cache = cache
        ?? throw new ArgumentNullException(nameof(cache));

    private readonly HackerNewsSettings _settings = settings
        ?? throw new ArgumentNullException(nameof(settings));

    private readonly ILogger<HackerNewsClient> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    private const string BestStoriesIdsCacheKey = "hacker_news_best_stories_ids";
    private const string StoryCacheKeyPrefix = "hacker_news_story_";

    /// <inheritdoc />
    public async Task<int[]> GetBestStoryIdsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(BestStoriesIdsCacheKey, out int[]? cachedIds) && cachedIds?.Length > 0)
        {
            return cachedIds;
        }

        try
        {
            var response = await _httpClient.GetAsync("beststories.json", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HackerNewsApiException(
                    $"Failed to retrieve best story IDs. Status code: {response.StatusCode}");
            }

            var ids = await response.Content.ReadFromJsonAsync<int[]>(cancellationToken: cancellationToken);

            if (ids is null || ids.Length == 0)
            {
                throw new HackerNewsApiException("Best stories list is empty or invalid.");
            }

            _cache.Set(
                BestStoriesIdsCacheKey,
                ids,
                TimeSpan.FromMinutes(_settings.BestStoriesIdsCacheDurationMinutes));

            return ids;
        }
        catch (HttpRequestException ex)
        {
            throw new HackerNewsApiException("Network error while retrieving best story IDs.", ex);
        }
        catch (JsonException ex)
        {
            throw new HackerNewsApiException("Invalid JSON response when retrieving best story IDs.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<HackerNewsStory?> GetStoryAsync(int storyId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{StoryCacheKeyPrefix}{storyId}";

        if (_cache.TryGetValue(cacheKey, out HackerNewsStory? cachedStory) && cachedStory != null)
        {
            return cachedStory;
        }

        try
        {
            var response = await _httpClient.GetAsync($"item/{storyId}.json", cancellationToken);

            // Return null for not found or other non-success responses
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve story with ID {StoryId}. Status code: {StatusCode}", storyId, response.StatusCode);
                return null;
            }

            var story = await response.Content.ReadFromJsonAsync<HackerNewsStory>(cancellationToken: cancellationToken);

            if (story != null)
            {
                _cache.Set(
                    cacheKey,
                    story,
                    TimeSpan.FromHours(_settings.StoryCacheDurationHours));
            }

            return story;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while retrieving story with ID {StoryId}", storyId);
            // Network errors result in null to allow graceful degradation
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error while retrieving story with ID {StoryId}", storyId);
            // JSON parsing errors result in null
            return null;
        }
    }
}