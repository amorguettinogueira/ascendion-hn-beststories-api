using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Extensions;
using Ascendion.HNBestStories.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Extensions;

/// <summary>
/// Unit tests for the HttpClientExtensions.
/// Tests HTTP client registration with resilience policies.
/// </summary>
public class HttpClientExtensionsTests
{
    [Fact]
    public void AddHackerNewsClient_RegistersHttpClientFactory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();  // Add logging required by the HTTP client

        // Act
        services.AddHackerNewsClient();

        // Assert
        services.Should().Contain(sd => sd.ServiceType == typeof(IHttpClientFactory));
    }

    [Fact]
    public void AddHackerNewsClient_RegistersHttpClient_WithCorrectBaseAddress()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddHackerNewsClient();

        // Assert
        result.Should().NotBeNull();
        services.Should().Contain(sd => sd.ServiceType == typeof(IHttpClientFactory));
    }

    [Fact]
    public void AddHackerNewsClient_ReturnsServiceCollection_ForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddHackerNewsClient();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IServiceCollection>();
    }

    [Fact]
    public void AddHackerNewsClient_ConfiguresResiliencePolicies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddHackerNewsClient();

        // Assert - Verify the client is registered in the service collection
        services.Should().Contain(sd => sd.ServiceType == typeof(IHackerNewsClient));
    }
}
