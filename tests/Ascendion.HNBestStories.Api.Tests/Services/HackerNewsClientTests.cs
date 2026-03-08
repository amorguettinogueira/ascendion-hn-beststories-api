using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Exceptions;
using Ascendion.HNBestStories.Api.Services;
using Ascendion.HNBestStories.Api.Settings;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Services;

/// <summary>
/// Unit tests for the HackerNewsClient.
/// Tests HTTP communication and caching behavior.
/// </summary>
public class HackerNewsClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly HackerNewsSettings _settings;
    private readonly Mock<ILogger<HackerNewsClient>> _mockLogger;
    private readonly IHackerNewsClient _client;

    public HackerNewsClientTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
        };
        _cache = new MemoryCache(new MemoryCacheOptions());
        _settings = new HackerNewsSettings
        {
            BestStoriesIdsCacheDurationMinutes = 5,
            StoryCacheDurationHours = 1
        };
        _mockLogger = new Mock<ILogger<HackerNewsClient>>();
        _client = new HackerNewsClient(_httpClient, _cache, _settings, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new HackerNewsClient(null!, _cache, _settings, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullCache_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new HackerNewsClient(_httpClient, null!, _settings, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new HackerNewsClient(_httpClient, _cache, null!, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new HackerNewsClient(_httpClient, _cache, _settings, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task GetBestStoryIdsAsync_WithSuccessResponse_ReturnsIds()
    {
        // Arrange
        var responseContent = "[1, 2, 3, 4, 5]";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _client.GetBestStoryIdsAsync();

        // Assert
        result.Should().HaveCount(5);
        result.Should().ContainInOrder(1, 2, 3, 4, 5);
    }

    [Fact]
    public async Task GetBestStoryIdsAsync_WithEmptyArray_ReturnsEmpty()
    {
        // Arrange
        var responseContent = "[]";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var act = () => _client.GetBestStoryIdsAsync();
        await act.Should().ThrowAsync<HackerNewsApiException>();
    }

    [Fact]
    public async Task GetBestStoryIdsAsync_WithFailure_ThrowsException()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var act = () => _client.GetBestStoryIdsAsync();
        await act.Should().ThrowAsync<HackerNewsApiException>();
    }

    [Fact]
    public async Task GetBestStoryIdsAsync_CachesResult()
    {
        // Arrange
        var responseContent = "[1, 2, 3]";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result1 = await _client.GetBestStoryIdsAsync();
        var result2 = await _client.GetBestStoryIdsAsync();

        // Assert
        result1.Should().Equal(result2);
        // Verify the HTTP handler was only called once (second call used cache)
        _mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetStoryAsync_WithValidStory_ReturnsStory()
    {
        // Arrange
        var storyJson = @"{
            ""id"": 1,
            ""title"": ""Test Story"",
            ""url"": ""http://example.com"",
            ""by"": ""TestUser"",
            ""time"": 1609459200,
            ""score"": 100,
            ""descendants"": 50
        }";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(storyJson)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _client.GetStoryAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be("Test Story");
        result.Score.Should().Be(100);
    }

    [Fact]
    public async Task GetStoryAsync_WithNotFound_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _client.GetStoryAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetStoryAsync_CachesResult()
    {
        // Arrange
        var storyJson = @"{""id"": 1, ""title"": ""Story"", ""url"": ""http://example.com"", ""by"": ""User"", ""time"": 1609459200, ""score"": 100, ""descendants"": 50}";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(storyJson)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result1 = await _client.GetStoryAsync(1);
        var result2 = await _client.GetStoryAsync(1);

        // Assert
        result1.Should().Be(result2);
        // Verify the HTTP handler was only called once
        _mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetStoryAsync_WithNetworkError_ReturnsNull()
    {
        // Arrange
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _client.GetStoryAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBestStoryIdsAsync_WithJsonException_ThrowsHackerNewsApiException()
    {
        // Arrange
        var responseContent = "{ invalid json }";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        var act = () => _client.GetBestStoryIdsAsync();
        await act.Should().ThrowAsync<HackerNewsApiException>();
    }

    [Fact]
    public async Task GetStoryAsync_WithJsonException_ReturnsNull()
    {
        // Arrange
        var invalidJson = "{ invalid json content }";
        var response = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(invalidJson)
        };

        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _client.GetStoryAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBestStoryIdsAsync_WithHttpRequestException_ThrowsHackerNewsApiException()
    {
        // Arrange
        _mockHttpHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection timeout"));

        // Act & Assert
        var act = () => _client.GetBestStoryIdsAsync();
        await act.Should().ThrowAsync<HackerNewsApiException>();
    }
}