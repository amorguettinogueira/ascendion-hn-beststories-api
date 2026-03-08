using Ascendion.HNBestStories.Api.Settings;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Settings;

/// <summary>
/// Unit tests for HackerNewsSettings configuration.
/// Tests default values, validation ranges, and property access.
/// </summary>
public class HackerNewsSettingsTests
{
    [Fact]
    public void DefaultValues_AreSet()
    {
        // Arrange & Act
        var settings = new HackerNewsSettings();

        // Assert
        settings.BestStoriesIdsCacheDurationMinutes.Should().Be(5);
        settings.StoryCacheDurationHours.Should().Be(1);
        settings.MaxStoriesAllowed.Should().Be(30);
        settings.MaxParallelRequests.Should().Be(10);
        settings.RequestTimeoutSeconds.Should().Be(2);
        settings.MaxRetryAttempts.Should().Be(3);
        settings.RetryDelayMilliseconds.Should().Be(200);
        settings.CircuitBreakerFailureRatio.Should().Be(0.5);
        settings.CircuitBreakerSamplingDurationSeconds.Should().Be(30);
        settings.CircuitBreakerMinimumThroughput.Should().Be(5);
    }

    [Fact]
    public void SectionName_IsCorrect()
    {
        // Assert
        HackerNewsSettings.SectionName.Should().Be("HackerNews");
    }

    [Fact]
    public void Properties_CanBeModified()
    {
        // Arrange
        var settings = new HackerNewsSettings();

        // Act
        settings.MaxStoriesAllowed = 50;
        settings.MaxParallelRequests = 20;
        settings.RequestTimeoutSeconds = 5;

        // Assert
        settings.MaxStoriesAllowed.Should().Be(50);
        settings.MaxParallelRequests.Should().Be(20);
        settings.RequestTimeoutSeconds.Should().Be(5);
    }

    [Fact]
    public void CacheDurationMinutes_HasValidRange()
    {
        // Arrange
        var settings = new HackerNewsSettings();
        var property = typeof(HackerNewsSettings).GetProperty(nameof(HackerNewsSettings.BestStoriesIdsCacheDurationMinutes));
        var rangeAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RangeAttribute), false).FirstOrDefault();

        // Assert
        rangeAttribute.Should().NotBeNull();
    }

    [Fact]
    public void MaxStoriesAllowed_HasValidRange()
    {
        // Arrange
        var settings = new HackerNewsSettings();
        var property = typeof(HackerNewsSettings).GetProperty(nameof(HackerNewsSettings.MaxStoriesAllowed));
        var rangeAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RangeAttribute), false).FirstOrDefault();

        // Assert
        rangeAttribute.Should().NotBeNull();
    }

    [Fact]
    public void MaxParallelRequests_HasValidRange()
    {
        // Arrange
        var property = typeof(HackerNewsSettings).GetProperty(nameof(HackerNewsSettings.MaxParallelRequests));
        var rangeAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RangeAttribute), false).FirstOrDefault();

        // Assert
        rangeAttribute.Should().NotBeNull();
    }

    [Fact]
    public void RequestTimeoutSeconds_HasValidRange()
    {
        // Arrange
        var property = typeof(HackerNewsSettings).GetProperty(nameof(HackerNewsSettings.RequestTimeoutSeconds));
        var rangeAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RangeAttribute), false).FirstOrDefault();

        // Assert
        rangeAttribute.Should().NotBeNull();
    }

    [Theory]
    [InlineData(nameof(HackerNewsSettings.MaxRetryAttempts))]
    [InlineData(nameof(HackerNewsSettings.RetryDelayMilliseconds))]
    [InlineData(nameof(HackerNewsSettings.CircuitBreakerFailureRatio))]
    [InlineData(nameof(HackerNewsSettings.CircuitBreakerSamplingDurationSeconds))]
    [InlineData(nameof(HackerNewsSettings.CircuitBreakerMinimumThroughput))]
    public void ResilienceProperties_HaveValidationAttributes(string propertyName)
    {
        // Arrange
        var property = typeof(HackerNewsSettings).GetProperty(propertyName);
        var rangeAttribute = property!.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RangeAttribute), false).FirstOrDefault();

        // Assert
        rangeAttribute.Should().NotBeNull($"{propertyName} should have Range validation");
    }
}
