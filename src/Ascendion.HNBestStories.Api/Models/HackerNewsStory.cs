using System.Text.Json.Serialization;

namespace Ascendion.HNBestStories.Api.Models;

/// <summary>
/// Represents a story object from the Hacker News API.
/// Maps to the JSON structure returned by the Hacker News API with proper JSON property naming.
/// </summary>
public sealed class HackerNewsStory
{
    /// <summary>
    /// Gets or sets the unique identifier of the story.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the story.
    /// May be null if not provided by the API.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the URL of the story.
    /// Maps to 'url' in the JSON response.
    /// May be null for text posts without a URL.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Uri { get; set; }

    /// <summary>
    /// Gets or sets the username of the story author.
    /// Maps to 'by' in the JSON response.
    /// May be null if not provided by the API.
    /// </summary>
    [JsonPropertyName("by")]
    public string? PostedBy { get; set; }

    /// <summary>
    /// Gets or sets the Unix timestamp when the story was posted.
    /// </summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>
    /// Gets or sets the score of the story (number of upvotes).
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the number of comments on the story.
    /// Maps to 'descendants' in the JSON response.
    /// </summary>
    [JsonPropertyName("descendants")]
    public int CommentCount { get; set; }

    /// <summary>
    /// Gets or sets the type of the item (e.g., "story", "comment").
    /// May be null if not provided by the API.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}