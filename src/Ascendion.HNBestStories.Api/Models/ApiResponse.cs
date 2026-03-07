namespace Ascendion.HNBestStories.Api.Models;

/// <summary>
/// Represents the result of an API operation using discriminated union pattern.
/// Provides type-safe error handling without using exceptions for expected failures.
/// </summary>
public abstract record ApiResponse
{
    /// <summary>
    /// Represents a successful API response containing the requested data.
    /// </summary>
    /// <param name="Data">The collection of best stories returned from the API.</param>
    public sealed record Success(IEnumerable<BestStory> Data) : ApiResponse;

    /// <summary>
    /// Represents an error API response with a descriptive message.
    /// </summary>
    /// <param name="Message">A human-readable error message describing what went wrong.</param>
    public sealed record Error(string Message) : ApiResponse;
}