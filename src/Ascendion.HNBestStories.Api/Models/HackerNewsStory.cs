using System.Text.Json.Serialization;

namespace Ascendion.HNBestStories.Api.Models;

/// <summary>
/// Represents a story from the Hacker News API
/// </summary>
public class HackerNewsStory
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Uri { get; set; }

    [JsonPropertyName("by")]
    public string? PostedBy { get; set; }

    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("descendants")]
    public int CommentCount { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}