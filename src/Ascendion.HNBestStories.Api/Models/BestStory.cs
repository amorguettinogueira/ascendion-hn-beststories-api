namespace Ascendion.HNBestStories.Api.Models;

/// <summary>
/// Represents the API response structure for best stories
/// </summary>
public class BestStory
{
    public string Title { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string PostedBy { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
}
