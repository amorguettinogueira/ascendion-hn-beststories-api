using Ascendion.HNBestStories.Api.Models;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Models;

/// <summary>
/// Unit tests for the BestStory record.
/// Tests immutability, validation, and null handling.
/// </summary>
public class BestStoryTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        // Arrange
        var title = "Test Story";
        var uri = "https://example.com";
        var postedBy = "TestUser";
        var time = DateTime.UtcNow;
        var score = 100;
        var commentCount = 50;

        // Act
        var story = new BestStory(title, uri, postedBy, time, score, commentCount);

        // Assert
        story.Title.Should().Be(title);
        story.Uri.Should().Be(uri);
        story.PostedBy.Should().Be(postedBy);
        story.Time.Should().Be(time);
        story.Score.Should().Be(score);
        story.CommentCount.Should().Be(commentCount);
    }

    [Fact]
    public void Constructor_WithNullTitle_UsesDefaultMessage()
    {
        // Arrange & Act
        var story = new BestStory(null!, "http://example.com", "User", DateTime.UtcNow, 10, 5);

        // Assert
        story.Title.Should().Be("No title available");
    }

    [Fact]
    public void Constructor_WithEmptyTitle_UsesDefaultMessage()
    {
        // Arrange & Act
        var story = new BestStory(string.Empty, "http://example.com", "User", DateTime.UtcNow, 10, 5);

        // Assert
        story.Title.Should().Be("No title available");
    }

    [Fact]
    public void Constructor_WithWhitespaceTitle_UsesDefaultMessage()
    {
        // Arrange & Act
        var story = new BestStory("   ", "http://example.com", "User", DateTime.UtcNow, 10, 5);

        // Assert
        story.Title.Should().Be("No title available");
    }

    [Fact]
    public void Constructor_WithNullPostedBy_UsesDefaultMessage()
    {
        // Arrange & Act
        var story = new BestStory("Title", "http://example.com", null!, DateTime.UtcNow, 10, 5);

        // Assert
        story.PostedBy.Should().Be("No author available");
    }

    [Fact]
    public void Constructor_WithEmptyPostedBy_UsesDefaultMessage()
    {
        // Arrange & Act
        var story = new BestStory("Title", "http://example.com", string.Empty, DateTime.UtcNow, 10, 5);

        // Assert
        story.PostedBy.Should().Be("No author available");
    }

    [Fact]
    public void Constructor_WithNullUri_UsesEmptyString()
    {
        // Arrange & Act
        var story = new BestStory("Title", null!, "Author", DateTime.UtcNow, 10, 5);

        // Assert
        story.Uri.Should().Be(string.Empty);
    }

    [Fact]
    public void IsValid_WithValidData_ReturnsTrue()
    {
        // Arrange
        var story = new BestStory("Valid Title", "http://example.com", "Author", DateTime.UtcNow, 10, 5);

        // Act
        var result = story.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithNullTitle_ReturnsTrue()
    {
        // Arrange
        var story = new BestStory(null!, "http://example.com", "Author", DateTime.UtcNow, 10, 5);

        // Act
        var result = story.IsValid();

        // Assert
        result.Should().BeTrue();
        story.Title.Should().Be("No title available");
    }

    [Fact]
    public void IsValid_WithNegativeScore_ReturnsFalse()
    {
        // Arrange
        var story = new BestStory("Title", "http://example.com", "Author", DateTime.UtcNow, -1, 5);

        // Act
        var result = story.IsValid();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithZeroScore_ReturnsTrue()
    {
        // Arrange
        var story = new BestStory("Title", "http://example.com", "Author", DateTime.UtcNow, 0, 5);

        // Act
        var result = story.IsValid();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Record_WithSameValues_AreEqual()
    {
        // Arrange
        var story1 = new BestStory("Title", "http://example.com", "Author", DateTime.UnixEpoch, 10, 5);
        var story2 = new BestStory("Title", "http://example.com", "Author", DateTime.UnixEpoch, 10, 5);

        // Act & Assert
        story1.Should().Be(story2);
    }

    [Fact]
    public void Record_WithDifferentValues_AreNotEqual()
    {
        // Arrange
        var story1 = new BestStory("Title1", "http://example.com", "Author", DateTime.UnixEpoch, 10, 5);
        var story2 = new BestStory("Title2", "http://example.com", "Author", DateTime.UnixEpoch, 10, 5);

        // Act & Assert
        story1.Should().NotBe(story2);
    }

    [Fact]
    public void Record_IsImmutable()
    {
        // Arrange
        var story = new BestStory("Title", "http://example.com", "Author", DateTime.UtcNow, 10, 5);

        // Act & Assert
        // Records are immutable - this test verifies they don't have setters
        var properties = typeof(BestStory).GetProperties();
        var settableProperties = properties.Where(p => p.SetMethod?.IsPublic == true).ToList();

        settableProperties.Should().BeEmpty("records should be immutable");
    }
}