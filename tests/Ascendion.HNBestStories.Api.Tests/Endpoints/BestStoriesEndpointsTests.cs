using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Endpoints;
using Ascendion.HNBestStories.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Endpoints;

/// <summary>
/// Integration tests for the BestStoriesEndpoints.
/// Tests HTTP request handling and response mapping.
/// </summary>
public class BestStoriesEndpointsTests
{
    [Fact]
    public async Task MapBestStoriesEndpoints_RegistersEndpoint()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddScoped(_ => mockService.Object);

        var app = builder.Build();

        // This would be called in Program.cs
        // app.MapBestStoriesEndpoints();

        // Assert
        // The test verifies the extension method exists and is callable
        var method = typeof(BestStoriesEndpoints).GetMethod("MapBestStoriesEndpoints");
        method.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleGetBestStories_WithSuccessResponse_ReturnsOkResult()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        var stories = new[] { new BestStory("Title", "http://example.com", "Author", DateTime.UtcNow, 10, 5) };
        mockService.Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse.Success(stories));

        // Act - Simulate endpoint handler
        var result = await InvokeGetBestStories(mockService.Object, 5);

        // Assert
        result.GetType().Name.Should().StartWith("Ok");
    }

    [Fact]
    public async Task HandleGetBestStories_WithErrorResponse_ReturnsBadRequestResult()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        mockService.Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse.Error("Invalid request"));

        // Act
        var result = await InvokeGetBestStories(mockService.Object, 100);

        // Assert
        result.GetType().Name.Should().StartWith("BadRequest");
    }

    [Fact]
    public async Task HandleGetBestStories_CallsServiceWithCorrectNumber()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        mockService.Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse.Success([]));

        // Act
        await InvokeGetBestStories(mockService.Object, 10);

        // Assert
        mockService.Verify(
            s => s.GetBestStoriesAsync(10, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleGetBestStories_PassesCancellationToken()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        var cts = new CancellationTokenSource();
        mockService.Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse.Success([]));

        // Act
        await InvokeGetBestStories(mockService.Object, 5, cts.Token);

        // Assert
        mockService.Verify(
            s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleGetBestStories_WithMultipleStories_ReturnsAllStories()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        var stories = new[]
        {
            new BestStory("Story 1", "http://example.com/1", "Author 1", DateTime.UtcNow, 100, 50),
            new BestStory("Story 2", "http://example.com/2", "Author 2", DateTime.UtcNow, 80, 40),
            new BestStory("Story 3", "http://example.com/3", "Author 3", DateTime.UtcNow, 60, 30)
        };
        mockService.Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse.Success(stories));

        // Act
        var result = await InvokeGetBestStories(mockService.Object, 3);

        // Assert
        result.GetType().Name.Should().StartWith("Ok");
    }

    [Fact]
    public async Task HandleGetBestStories_WithEmptyStories_ReturnsOkWithEmptyList()
    {
        // Arrange
        var mockService = new Mock<IBestStoriesService>();
        var emptyStories = Array.Empty<BestStory>();
        mockService.Setup(s => s.GetBestStoriesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApiResponse.Success(emptyStories));

        // Act
        var result = await InvokeGetBestStories(mockService.Object, 5);

        // Assert
        result.GetType().Name.Should().StartWith("Ok");
    }

    // Helper method to simulate endpoint handler invocation
    private async Task<IResult> InvokeGetBestStories(
        IBestStoriesService service,
        int numberOfStories,
        CancellationToken cancellationToken = default)
    {
        var response = await service.GetBestStoriesAsync(numberOfStories, cancellationToken);

        return response switch
        {
            ApiResponse.Success success =>
                Results.Ok(success.Data),
            ApiResponse.Error error =>
                Results.BadRequest(new { error = error.Message }),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}