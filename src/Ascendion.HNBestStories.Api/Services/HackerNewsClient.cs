using Microsoft.Extensions.Caching.Memory;
using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Models;
using Ascendion.HNBestStories.Api.Settings;

namespace Ascendion.HNBestStories.Api.Services;

public class HackerNewsClient(HttpClient httpClient, IMemoryCache cache, HackerNewsSettings settings) : IHackerNewsClient
{
    private readonly HttpClient _httpClient = httpClient
        ?? throw new ArgumentNullException(nameof(httpClient));

    private readonly IMemoryCache _cache = cache
        ?? throw new ArgumentNullException(nameof(cache));

    private readonly HackerNewsSettings _settings = settings
        ?? throw new ArgumentNullException(nameof(settings));

    private const string BestStoriesIdsCacheKey = "hacker_news_best_stories_ids";
    private const string StoryCacheKeyPrefix = "hacker_news_story_";

    public async Task<int[]> GetBestStoryIdsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(BestStoriesIdsCacheKey, out int[]? cachedIds) && cachedIds?.Length > 0)
        {
            return cachedIds;
        }

        var response = await _httpClient.GetAsync("beststories.json", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var ids = await response.Content.ReadFromJsonAsync<int[]>(cancellationToken);

        _ = _cache.Set(BestStoriesIdsCacheKey, ids, TimeSpan.FromMinutes(_settings.BestStoriesIdsCacheDurationMinutes));

        return ids ?? [];
    }

    public async Task<HackerNewsStory?> GetStoryAsync(int storyId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{StoryCacheKeyPrefix}{storyId}";

        if (_cache.TryGetValue(cacheKey, out HackerNewsStory? cachedStory) && cachedStory != null)
        {
            return cachedStory;
        }

        var response = await _httpClient.GetAsync($"item/{storyId}.json", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var story = await response.Content.ReadFromJsonAsync<HackerNewsStory>(cancellationToken);

        _ = _cache.Set(cacheKey, story, TimeSpan.FromHours(_settings.StoryCacheDurationHours));

        return story;
    }
}