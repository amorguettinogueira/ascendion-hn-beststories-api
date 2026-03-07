using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Abstractions;

/// <summary>
/// Defines the contract for retrieving best stories from Hacker News.
/// Implementations should handle caching, resilience, and error scenarios.
/// </summary>
public interface IBestStoriesService
{
    /// <summary>
    /// Retrieves the top N best stories from Hacker News, sorted by score in descending order.
    /// </summary>
    /// <param name="numberOfStories">
    /// The number of best stories to retrieve.
    /// Validation of the range is performed by the implementation.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// An API response that is either a Success containing the requested stories,
    /// or an Error with a descriptive message if something went wrong.
    /// </returns>
    Task<ApiResponse> GetBestStoriesAsync(int numberOfStories, CancellationToken cancellationToken = default);
}