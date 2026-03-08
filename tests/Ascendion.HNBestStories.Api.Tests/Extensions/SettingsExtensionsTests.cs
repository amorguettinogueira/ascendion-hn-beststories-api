using Ascendion.HNBestStories.Api.Extensions;
using Ascendion.HNBestStories.Api.Settings;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Extensions;

/// <summary>
/// Unit tests for the SettingsExtensions.
/// Tests application settings configuration and validation.
/// </summary>
public class SettingsExtensionsTests
{
    [Fact]
    public void AddApplicationSettings_WithValidConfiguration_RegistersSettings()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "HackerNews:MaxStoriesAllowed", "30" },
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:RequestTimeoutSeconds", "2" },
                { "HackerNews:MaxRetryAttempts", "3" },
                { "HackerNews:RetryDelayMilliseconds", "200" },
                { "HackerNews:CircuitBreakerFailureRatio", "0.5" },
                { "HackerNews:CircuitBreakerSamplingDurationSeconds", "30" },
                { "HackerNews:CircuitBreakerMinimumThroughput", "5" },
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationSettings(config);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(services);
        var provider = services.BuildServiceProvider();
        var hackerNewsSettings = provider.GetRequiredService<HackerNewsSettings>();
        hackerNewsSettings.Should().NotBeNull();
        hackerNewsSettings.BestStoriesIdsCacheDurationMinutes.Should().Be(5);
        hackerNewsSettings.StoryCacheDurationHours.Should().Be(1);
        hackerNewsSettings.MaxStoriesAllowed.Should().Be(30);
        hackerNewsSettings.MaxParallelRequests.Should().Be(10);
        hackerNewsSettings.RequestTimeoutSeconds.Should().Be(2);
        hackerNewsSettings.MaxRetryAttempts.Should().Be(3);
        hackerNewsSettings.RetryDelayMilliseconds.Should().Be(200);
        hackerNewsSettings.CircuitBreakerFailureRatio.Should().Be(0.5);
        hackerNewsSettings.CircuitBreakerSamplingDurationSeconds.Should().Be(30);
        hackerNewsSettings.CircuitBreakerMinimumThroughput.Should().Be(5);
    }

    [Fact]
    public void AddApplicationSettings_WithInvalidHackerNewsSettings_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "HackerNews:MaxStoriesAllowed", "-5" },  // Invalid: negative value
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:RequestTimeoutSeconds", "2" },
                { "HackerNews:MaxRetryAttempts", "3" },
                { "HackerNews:RetryDelayMilliseconds", "200" },
                { "HackerNews:CircuitBreakerFailureRatio", "0.5" },
                { "HackerNews:CircuitBreakerSamplingDurationSeconds", "30" },
                { "HackerNews:CircuitBreakerMinimumThroughput", "5" },
            })
            .Build();

        var services = new ServiceCollection();

        // Act & Assert
        var act = () => services.AddApplicationSettings(config);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*HackerNews settings validation failed*");
    }

    [Fact]
    public void AddApplicationSettings_ReturnsServiceCollection_ForChaining()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "HackerNews:MaxStoriesAllowed", "30" },
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:RequestTimeoutSeconds", "2" },
                { "HackerNews:MaxRetryAttempts", "3" },
                { "HackerNews:RetryDelayMilliseconds", "200" },
                { "HackerNews:CircuitBreakerFailureRatio", "0.5" },
                { "HackerNews:CircuitBreakerSamplingDurationSeconds", "30" },
                { "HackerNews:CircuitBreakerMinimumThroughput", "5" },
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        var result = services.AddApplicationSettings(config);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(services);
    }
}