using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Abstractions;

/// <summary>
/// Defines the contract for communicating with the Hacker News API.
/// Implementations should handle resilience, caching, and errors appropriately.
/// </summary>
public interface IHackerNewsClient
{
    /// <summary>
    /// Gets the IDs of the best stories from Hacker News.
    /// Results are typically cached to reduce load on the upstream service.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// An array of story IDs. If retrieval fails, an exception may be thrown
    /// or an empty array may be returned, depending on the implementation.
    /// </returns>
    /// <exception cref="Exception">
    /// May throw exceptions if the request fails in an unrecoverable way.
    /// </exception>
    Task<int[]> GetBestStoryIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the details of a story by its ID.
    /// Results are typically cached to reduce API load.
    /// </summary>
    /// <param name="storyId">
    /// The unique identifier of the story to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// The story details if found; null if the story doesn't exist or cannot be retrieved
    /// (e.g., network failure, API error).
    /// </returns>
    Task<HackerNewsStory?> GetStoryAsync(int storyId, CancellationToken cancellationToken = default);
}