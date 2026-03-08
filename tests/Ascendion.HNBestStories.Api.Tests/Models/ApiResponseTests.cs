using Ascendion.HNBestStories.Api.Models;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Models;

/// <summary>
/// Unit tests for the ApiResponse discriminated union.
/// Tests both Success and Error record types.
/// </summary>
public class ApiResponseTests
{
    [Fact]
    public void Success_WithData_CreatesSuccessResponse()
    {
        // Arrange
        var stories = new[]
        {
            new BestStory("Story 1", "http://example.com", "Author", DateTime.UtcNow, 10, 5),
            new BestStory("Story 2", "http://example.com", "Author", DateTime.UtcNow, 20, 10)
        };

        // Act
        ApiResponse response = new ApiResponse.Success(stories);

        // Assert
        response.Should().BeOfType<ApiResponse.Success>();
        ((ApiResponse.Success)response).Data.Should().HaveCount(2);
    }

    [Fact]
    public void Success_WithEmptyData_CreatesSuccessResponse()
    {
        // Arrange
        var stories = Array.Empty<BestStory>();

        // Act
        ApiResponse response = new ApiResponse.Success(stories);

        // Assert
        response.Should().BeOfType<ApiResponse.Success>();
        ((ApiResponse.Success)response).Data.Should().BeEmpty();
    }

    [Fact]
    public void Error_WithMessage_CreatesErrorResponse()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        ApiResponse response = new ApiResponse.Error(errorMessage);

        // Assert
        response.Should().BeOfType<ApiResponse.Error>();
        ((ApiResponse.Error)response).Message.Should().Be(errorMessage);
    }

    [Fact]
    public void PatternMatching_WithSuccess_MatchesCorrectly()
    {
        // Arrange
        var stories = new[] { new BestStory("Story", "http://example.com", "Author", DateTime.UtcNow, 10, 5) };
        ApiResponse response = new ApiResponse.Success(stories);

        // Act
        var matched = response switch
        {
            ApiResponse.Success success => "success",
            ApiResponse.Error => "error",
            _ => "unknown"
        };

        // Assert
        matched.Should().Be("success");
    }

    [Fact]
    public void PatternMatching_WithError_MatchesCorrectly()
    {
        // Arrange
        ApiResponse response = new ApiResponse.Error("Error message");

        // Act
        var matched = response switch
        {
            ApiResponse.Success => "success",
            ApiResponse.Error error => error.Message,
            _ => "unknown"
        };

        // Assert
        matched.Should().Be("Error message");
    }

    [Fact]
    public void Success_WithSameData_AreEqual()
    {
        // Arrange
        var stories = new[] { new BestStory("Story", "http://example.com", "Author", DateTime.UnixEpoch, 10, 5) };
        var response1 = new ApiResponse.Success(stories);
        var response2 = new ApiResponse.Success(stories);

        // Act & Assert
        response1.Should().Be(response2);
    }

    [Fact]
    public void Error_WithSameMessage_AreEqual()
    {
        // Arrange
        var response1 = new ApiResponse.Error("Error message");
        var response2 = new ApiResponse.Error("Error message");

        // Act & Assert
        response1.Should().Be(response2);
    }

    [Fact]
    public void Error_WithDifferentMessages_AreNotEqual()
    {
        // Arrange
        var response1 = new ApiResponse.Error("Error 1");
        var response2 = new ApiResponse.Error("Error 2");

        // Act & Assert
        response1.Should().NotBe(response2);
    }
}
