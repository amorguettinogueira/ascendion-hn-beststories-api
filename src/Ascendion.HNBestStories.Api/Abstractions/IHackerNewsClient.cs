using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Abstractions;

public interface IHackerNewsClient
{
    /// <summary>
    /// Gets the IDs of the best stories from Hacker News.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Array of story IDs</returns>
    Task<int[]> GetBestStoryIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the details of a story by its ID.
    /// </summary>
    /// <param name="storyId">The story ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Story details, or null if not found</returns>
    Task<HackerNewsStory?> GetStoryAsync(int storyId, CancellationToken cancellationToken = default);
}