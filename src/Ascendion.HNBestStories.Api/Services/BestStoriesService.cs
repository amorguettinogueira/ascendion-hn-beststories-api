using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Exceptions;
using Ascendion.HNBestStories.Api.Models;
using Ascendion.HNBestStories.Api.Settings;
using Ascendion.HNBestStories.Api.Utilities;

namespace Ascendion.HNBestStories.Api.Services;

/// <inheritdoc />
public sealed class BestStoriesService(
    IHackerNewsClient hackerNewsClient,
    HackerNewsSettings settings,
    IStoriesRequestValidator validator) : IBestStoriesService
{
    private readonly IHackerNewsClient _hackerNewsClient = hackerNewsClient
        ?? throw new ArgumentNullException(nameof(hackerNewsClient));

    private readonly HackerNewsSettings _settings = settings
        ?? throw new ArgumentNullException(nameof(settings));

    private readonly IStoriesRequestValidator _validator = validator
        ?? throw new ArgumentNullException(nameof(validator));

    /// <inheritdoc />
    public async Task<ApiResponse> GetBestStoriesAsync(int numberOfStories, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate input using injected validator
            var validationError = _validator.Validate(numberOfStories);

            if (validationError is not null)
            {
                return new ApiResponse.Error(validationError);
            }

            // Get the list of best story IDs
            var bestStoryIds = await _hackerNewsClient.GetBestStoryIdsAsync(cancellationToken);

            if (bestStoryIds.Length == 0)
            {
                return new ApiResponse.Error("Something went wrong, unable to obtain the list of top stories from Hacker News.");
            }

            // Fetch and convert stories
            var bestStories = await FetchAndConvertStoriesAsync(bestStoryIds, numberOfStories, cancellationToken);

            return bestStories.Count == 0
                ? new ApiResponse.Error("Failed to retrieve any stories.")
                : new ApiResponse.Success(bestStories.OrderByDescending(story => story.Score));
        }
        catch (HackerNewsApiException ex)
        {
            return new ApiResponse.Error($"Hacker News API error: {ex.Message}");
        }
        catch (StoriesValidationException ex)
        {
            return new ApiResponse.Error($"Validation error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new ApiResponse.Error($"An unexpected error occurred while retrieving best stories: {ex.Message}");
        }
    }

    /// <summary>
    /// Fetches stories in parallel batches and converts them to BestStory DTOs.
    /// </summary>
    /// <param name="bestStoryIds">The array of story IDs to fetch.</param>
    /// <param name="numberOfStories">The total number of stories to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of converted BestStory objects.</returns>
    private async Task<List<BestStory>> FetchAndConvertStoriesAsync(
        int[] bestStoryIds,
        int numberOfStories,
        CancellationToken cancellationToken)
    {
        var bestStories = new List<BestStory>(capacity: numberOfStories);

        var startStoryIndex = 0;

        while (bestStories.Count < numberOfStories && startStoryIndex < bestStoryIds.Length)
        {
            // Calculate batch size
            var batchSize = Math.Min(
                _settings.MaxParallelRequests,
                numberOfStories - bestStories.Count);

            // Extract story IDs for this batch
            var storyIdsToFetch = bestStoryIds[startStoryIndex..(startStoryIndex + batchSize)];
            startStoryIndex += batchSize;

            // Fetch all stories in parallel
            var storyTasks = storyIdsToFetch.Select(id => _hackerNewsClient.GetStoryAsync(id, cancellationToken));
            var stories = await Task.WhenAll(storyTasks);

            // Convert and add valid stories to results
            bestStories.AddRange([..
                stories
                    .OfType<HackerNewsStory>()
                    .Select(ConvertToApiStory)
                    .Where(story => story.IsValid())
            ]);
        }

        return bestStories;
    }

    /// <summary>
    /// Converts a HackerNewsStory to a BestStory API response model.
    /// </summary>
    /// <param name="story">The Hacker News story to convert.</param>
    /// <returns>A BestStory model ready for API response.</returns>
    private static BestStory ConvertToApiStory(HackerNewsStory story) =>
        new(Title: story.Title ?? string.Empty,
            Uri: story.Uri ?? string.Empty,
            PostedBy: story.PostedBy ?? string.Empty,
            Time: DateTimeConverter.FromUnixTimeStamp(story.Time),
            Score: story.Score,
            CommentCount: story.CommentCount);
}