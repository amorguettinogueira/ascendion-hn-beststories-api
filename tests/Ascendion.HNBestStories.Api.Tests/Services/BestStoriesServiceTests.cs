using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Models;
using Ascendion.HNBestStories.Api.Services;
using Ascendion.HNBestStories.Api.Settings;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Services;

/// <summary>
/// Unit tests for the BestStoriesService.
/// Tests service orchestration with mocked dependencies.
/// </summary>
public class BestStoriesServiceTests
{
    private readonly Mock<IHackerNewsClient> _mockHackerNewsClient;
    private readonly Mock<IStoriesRequestValidator> _mockValidator;
    private readonly Mock<ILogger<BestStoriesService>> _mockLogger;
    private readonly HackerNewsSettings _settings;
    private readonly BestStoriesService _service;

    public BestStoriesServiceTests()
    {
        _mockHackerNewsClient = new Mock<IHackerNewsClient>();
        _mockValidator = new Mock<IStoriesRequestValidator>();
        _mockLogger = new Mock<ILogger<BestStoriesService>>();
        _settings = new HackerNewsSettings
        {
            MaxStoriesAllowed = 30,
            MaxParallelRequests = 10
        };
        _service = new BestStoriesService(
            _mockHackerNewsClient.Object,
            _settings,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new BestStoriesService(null!, _settings, _mockValidator.Object, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new BestStoriesService(_mockHackerNewsClient.Object, null!, _mockValidator.Object, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullValidator_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new BestStoriesService(_mockHackerNewsClient.Object, _settings, null!, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new BestStoriesService(_mockHackerNewsClient.Object, _settings, _mockValidator.Object, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithValidationError_ReturnsErrorResponse()
    {
        // Arrange
        _mockValidator.Setup(v => v.Validate(It.IsAny<int>()))
            .Returns("Validation error message");

        // Act
        var result = await _service.GetBestStoriesAsync(100);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("Validation error");
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithValidInput_CallsValidator()
    {
        // Arrange
        var numberOfStories = 5;
        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<int>());

        // Act
        await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        _mockValidator.Verify(v => v.Validate(numberOfStories), Times.Once);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithEmptyStoryIds_ReturnsErrorResponse()
    {
        // Arrange
        _mockValidator.Setup(v => v.Validate(It.IsAny<int>())).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<int>());

        // Act
        var result = await _service.GetBestStoriesAsync(5);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("unable to obtain");
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithValidStories_ReturnsSuccessResponse()
    {
        // Arrange
        var numberOfStories = 2;
        var storyIds = new[] { 1, 2 };
        var hackernewsStories = new[]
        {
            new HackerNewsStory { Id = 1, Title = "Story 1", Uri = "http://example.com", PostedBy = "User", Time = 1609459200, Score = 100, CommentCount = 50 },
            new HackerNewsStory { Id = 2, Title = "Story 2", Uri = "http://example.com", PostedBy = "User", Time = 1609459200, Score = 50, CommentCount = 25 }
        };

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storyIds);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hackernewsStories[0]);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hackernewsStories[1]);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Success>();
        var success = (ApiResponse.Success)result;
        success.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithPartialFailure_ReturnsAvailableStories()
    {
        // Arrange
        var numberOfStories = 2;
        var storyIds = new[] { 1, 2 };
        var hackernewsStory = new HackerNewsStory
        {
            Id = 1,
            Title = "Story 1",
            Uri = "http://example.com",
            PostedBy = "User",
            Time = 1609459200,
            Score = 100,
            CommentCount = 50
        };

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storyIds);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hackernewsStory);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as HackerNewsStory);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Success>();
        var success = (ApiResponse.Success)result;
        success.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithAllStoryFails_ReturnsErrorResponse()
    {
        // Arrange
        var numberOfStories = 2;
        var storyIds = new[] { 1, 2 };

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storyIds);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as HackerNewsStory);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("Failed to retrieve any stories");
    }

    [Fact]
    public async Task GetBestStoriesAsync_ReturnsSortedByScore()
    {
        // Arrange
        var numberOfStories = 3;
        var storyIds = new[] { 1, 2, 3 };
        var hackernewsStories = new[]
        {
            new HackerNewsStory { Id = 1, Title = "Story 1", Uri = "http://example.com", PostedBy = "User", Time = 1609459200, Score = 50, CommentCount = 10 },
            new HackerNewsStory { Id = 2, Title = "Story 2", Uri = "http://example.com", PostedBy = "User", Time = 1609459200, Score = 200, CommentCount = 20 },
            new HackerNewsStory { Id = 3, Title = "Story 3", Uri = "http://example.com", PostedBy = "User", Time = 1609459200, Score = 100, CommentCount = 30 }
        };

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storyIds);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hackernewsStories[0]);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hackernewsStories[1]);
        _mockHackerNewsClient.Setup(c => c.GetStoryAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hackernewsStories[2]);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Success>();
        var success = (ApiResponse.Success)result;
        var stories = success.Data.ToList();
        stories[0].Score.Should().Be(200);
        stories[1].Score.Should().Be(100);
        stories[2].Score.Should().Be(50);
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithCancellation_ReturnsErrorResponse()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockValidator.Setup(v => v.Validate(It.IsAny<int>())).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken ct) =>
            {
                cts.Cancel();
                await Task.Delay(100, ct);
                return [1, 2];
            });

        // Act
        var result = await _service.GetBestStoriesAsync(5, cts.Token);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("unexpected error");
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithHackerNewsApiException_ReturnsErrorResponse()
    {
        // Arrange
        var numberOfStories = 5;
        var exception = new Ascendion.HNBestStories.Api.Exceptions.HackerNewsApiException("API connection failed");

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("Hacker News API error");
        error.Message.Should().Contain("API connection failed");
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithStoriesValidationException_ReturnsErrorResponse()
    {
        // Arrange
        var numberOfStories = 5;
        var exception = new Ascendion.HNBestStories.Api.Exceptions.StoriesValidationException("Invalid story validation");

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("Validation error");
        error.Message.Should().Contain("Invalid story validation");
    }

    [Fact]
    public async Task GetBestStoriesAsync_WithGenericException_ReturnsErrorResponse()
    {
        // Arrange
        var numberOfStories = 5;
        var exception = new InvalidOperationException("Unexpected error occurred");

        _mockValidator.Setup(v => v.Validate(numberOfStories)).Returns(null as string);
        _mockHackerNewsClient.Setup(c => c.GetBestStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _service.GetBestStoriesAsync(numberOfStories);

        // Assert
        result.Should().BeOfType<ApiResponse.Error>();
        var error = (ApiResponse.Error)result;
        error.Message.Should().Contain("unexpected error");
        error.Message.Should().Contain("Unexpected error occurred");
    }
}