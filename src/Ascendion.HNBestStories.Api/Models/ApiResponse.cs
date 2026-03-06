namespace Ascendion.HNBestStories.Api.Models;

/// <summary>
/// Generic API response wrapper for consistent success/error handling
/// </summary>
public abstract record ApiResponse
{
    public sealed record Success(IEnumerable<BestStory> Data) : ApiResponse;
    public sealed record Error(string Message) : ApiResponse;
}
