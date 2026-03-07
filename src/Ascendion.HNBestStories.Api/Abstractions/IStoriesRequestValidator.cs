namespace Ascendion.HNBestStories.Api.Abstractions;

/// <summary>
/// Defines validation logic for story requests.
/// Separates validation concerns from business logic.
/// </summary>
public interface IStoriesRequestValidator
{
    /// <summary>
    /// Validates the requested number of stories.
    /// </summary>
    /// <param name="numberOfStories">The number of stories requested.</param>
    /// <returns>Null if valid; otherwise an error message.</returns>
    string? Validate(int numberOfStories);
}
