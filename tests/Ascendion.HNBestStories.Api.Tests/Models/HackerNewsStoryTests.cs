using Ascendion.HNBestStories.Api.Models;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Models;

/// <summary>
/// Unit tests for the HackerNewsStory class.
/// Tests JSON deserialization and property access.
/// </summary>
public class HackerNewsStoryTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesInstance()
    {
        // Arrange & Act
        var story = new HackerNewsStory
        {
            Id = 1,
            Title = "Test Story",
            Uri = "http://example.com",
            PostedBy = "TestUser",
            Time = 1609459200,
            Score = 100,
            CommentCount = 50,
            Type = "story"
        };

        // Assert
        story.Id.Should().Be(1);
        story.Title.Should().Be("Test Story");
        story.Uri.Should().Be("http://example.com");
        story.PostedBy.Should().Be("TestUser");
        story.Time.Should().Be(1609459200);
        story.Score.Should().Be(100);
        story.CommentCount.Should().Be(50);
        story.Type.Should().Be("story");
    }

    [Fact]
    public void Title_CanBeNull()
    {
        // Arrange & Act
        var story = new HackerNewsStory { Id = 1, Title = null };

        // Assert
        story.Title.Should().BeNull();
    }

    [Fact]
    public void Uri_CanBeNull()
    {
        // Arrange & Act
        var story = new HackerNewsStory { Id = 1, Uri = null };

        // Assert
        story.Uri.Should().BeNull();
    }

    [Fact]
    public void PostedBy_CanBeNull()
    {
        // Arrange & Act
        var story = new HackerNewsStory { Id = 1, PostedBy = null };

        // Assert
        story.PostedBy.Should().BeNull();
    }

    [Fact]
    public void Type_CanBeNull()
    {
        // Arrange & Act
        var story = new HackerNewsStory { Id = 1, Type = null };

        // Assert
        story.Type.Should().BeNull();
    }

    [Fact]
    public void AllProperties_AreAccessible()
    {
        // Arrange
        var story = new HackerNewsStory();

        // Act & Assert - Verify all properties are readable/writable
        story.Id = 123;
        story.Id.Should().Be(123);

        story.Title = "Title";
        story.Title.Should().Be("Title");

        story.Uri = "http://example.com";
        story.Uri.Should().Be("http://example.com");

        story.PostedBy = "User";
        story.PostedBy.Should().Be("User");

        story.Time = 1234567890;
        story.Time.Should().Be(1234567890);

        story.Score = 500;
        story.Score.Should().Be(500);

        story.CommentCount = 250;
        story.CommentCount.Should().Be(250);

        story.Type = "story";
        story.Type.Should().Be("story");
    }

    [Fact]
    public void Story_WithDefaultValues()
    {
        // Arrange & Act
        var story = new HackerNewsStory();

        // Assert
        story.Id.Should().Be(0);
        story.Title.Should().BeNull();
        story.Uri.Should().BeNull();
        story.PostedBy.Should().BeNull();
        story.Time.Should().Be(0);
        story.Score.Should().Be(0);
        story.CommentCount.Should().Be(0);
        story.Type.Should().BeNull();
    }
}
