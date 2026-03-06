using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Abstractions;

public interface IBestStoriesService
{
    /// <summary>
    /// Retrieves the top n best stories from Hacker News, sorted by score in descending order.
    /// </summary>
    /// <param name="numberOfStories">The number of best stories to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An API response containing the stories or error information</returns>
    Task<ApiResponse> GetBestStoriesAsync(int numberOfStories, CancellationToken cancellationToken = default);
}
