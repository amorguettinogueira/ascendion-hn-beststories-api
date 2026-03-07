namespace Ascendion.HNBestStories.Api.Models;

/// <summary>
/// Represents a best story from the Hacker News API.
/// Immutable record type ensuring data consistency and thread-safety.
/// </summary>
public sealed record BestStory(
    string Title,
    string Uri,
    string PostedBy,
    DateTime Time,
    int Score,
    int CommentCount)
{
    /// <summary>
    /// Gets the title of the story. Falls back to default if not provided.
    /// </summary>
    public string Title { get; } = string.IsNullOrWhiteSpace(Title) ? "No title available" : Title;

    /// <summary>
    /// Gets the URI of the story.
    /// </summary>
    public string Uri { get; } = Uri ?? string.Empty;

    /// <summary>
    /// Gets the author of the story. Falls back to default if not provided.
    /// </summary>
    public string PostedBy { get; } = string.IsNullOrWhiteSpace(PostedBy) ? "No author available" : PostedBy;

    /// <summary>
    /// Validates the story data.
    /// </summary>
    /// <returns>True if the story contains minimal valid data; otherwise false.</returns>
    public bool IsValid() => !string.IsNullOrWhiteSpace(Title) && Score >= 0;
}
