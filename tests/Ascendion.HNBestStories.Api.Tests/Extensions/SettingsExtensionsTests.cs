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
                { "HackerNews:MaxStoriesAllowed", "30" },
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "Api:ApiVersions:CurrentVersion", "1.0" },
                { "Api:ApiVersions:DeprecatedVersions", "" }
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
        hackerNewsSettings.MaxStoriesAllowed.Should().Be(30);
        hackerNewsSettings.MaxParallelRequests.Should().Be(10);
    }

    [Fact]
    public void AddApplicationSettings_WithInvalidHackerNewsSettings_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "HackerNews:MaxStoriesAllowed", "-5" },  // Invalid: negative value
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "Api:ApiVersions:CurrentVersion", "1.0" },
                { "Api:ApiVersions:DeprecatedVersions", "" }
            })
            .Build();

        var services = new ServiceCollection();

        // Act & Assert
        var act = () => services.AddApplicationSettings(config);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*HackerNews settings validation failed*");
    }

    [Fact]
    public void AddApplicationSettings_WithInvalidApiSettings_ThrowsInvalidOperationException()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "HackerNews:MaxStoriesAllowed", "30" },
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "Api:ApiVersions:CurrentVersion", "1.0" },
                { "Api:ApiVersions:DeprecatedVersions", "" }
            })
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddApplicationSettings(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var apiSettings = provider.GetRequiredService<ApiSettings>();
        apiSettings.Should().NotBeNull();
    }

    [Fact]
    public void AddApplicationSettings_ReturnsServiceCollection_ForChaining()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "HackerNews:MaxStoriesAllowed", "30" },
                { "HackerNews:MaxParallelRequests", "10" },
                { "HackerNews:BestStoriesIdsCacheDurationMinutes", "5" },
                { "HackerNews:StoryCacheDurationHours", "1" },
                { "Api:ApiVersions:CurrentVersion", "1.0" },
                { "Api:ApiVersions:DeprecatedVersions", "" }
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
