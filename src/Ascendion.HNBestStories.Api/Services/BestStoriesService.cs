using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Models;
using Ascendion.HNBestStories.Api.Settings;

namespace Ascendion.HNBestStories.Api.Services;

public class BestStoriesService(IHackerNewsClient hackerNewsClient, HackerNewsSettings settings) : IBestStoriesService
{
    private readonly IHackerNewsClient _hackerNewsClient = hackerNewsClient
        ?? throw new ArgumentNullException(nameof(hackerNewsClient));

    private readonly HackerNewsSettings _settings = settings
        ?? throw new ArgumentNullException(nameof(settings));

    public async Task<ApiResponse> GetBestStoriesAsync(int numberOfStories, CancellationToken cancellationToken = default)
    {
        // Validate input
        if (numberOfStories <= 0)
        {
            return new ApiResponse.Error("Number of stories must be greater than 0.");
        }

        if (numberOfStories > _settings.MaxStoriesAllowed)
        {
            return new ApiResponse.Error($"Number of stories must not exceed {_settings.MaxStoriesAllowed}.");
        }

        try
        {
            // Get the list of best story IDs
            var bestStoryIds = await _hackerNewsClient.GetBestStoryIdsAsync(cancellationToken);

            if (bestStoryIds.Length == 0)
            {
                return new ApiResponse.Error("Something went wrong, unable to obtain the list of top stories from Hacker News.");
            }

            var bestStories = new List<BestStory>();
            var startStoryIndex = 0;

            while (bestStories.Count < numberOfStories && startStoryIndex < bestStoryIds.Length)
            {
                // Calculate how many stories to fetch in this batch, ensuring we don't exceed the requested number
                var numberOfStoriesToFetch = Math.Min(_settings.MaxParallelRequests, numberOfStories - bestStories.Count);

                // Take only the requested number of IDs
                var storyIdsToFetch = bestStoryIds[startStoryIndex..(startStoryIndex + numberOfStoriesToFetch)];
                startStoryIndex += numberOfStoriesToFetch;

                // Fetch all stories in parallel
                var storyTasks = storyIdsToFetch.Select(id => _hackerNewsClient.GetStoryAsync(id, cancellationToken));
                var stories = await Task.WhenAll(storyTasks);

                // Filter out null stories (in case of failed requests) and convert to BestStory DTO
                bestStories.AddRange([..
                    stories
                        .Where(story => story != null)
                        .Select(story => new BestStory
                        {
                            Title = story!.Title ?? "No title available",
                            Uri = story.Uri ?? string.Empty,
                            PostedBy = story.PostedBy ?? "No author available",
                            Time = UnixTimeStampToDateTime(story.Time),
                            Score = story.Score,
                            CommentCount = story.CommentCount
                        })
                ]);
            }

            return bestStories.Count == 0
                ? new ApiResponse.Error("Failed to retrieve any stories.")
                : new ApiResponse.Success(bestStories.OrderByDescending(story => story.Score));
        }
        catch (Exception ex)
        {
            return new ApiResponse.Error($"An error occurred while retrieving best stories: {ex.Message}");
        }
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
    }
}