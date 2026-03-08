using Ascendion.HNBestStories.Api.Settings;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Settings;

/// <summary>
/// Unit tests for ApiSettings configuration.
/// Tests default values and property access.
/// </summary>
public class ApiSettingsTests
{
    [Fact]
    public void DefaultValues_AreSet()
    {
        // Arrange & Act
        var settings = new ApiSettings();

        // Assert
        settings.DevelopmentMode.Should().BeFalse();
        settings.DetailedErrorMessages.Should().BeFalse();
    }

    [Fact]
    public void SectionName_IsCorrect()
    {
        // Assert
        ApiSettings.SectionName.Should().Be("Api");
    }

    [Fact]
    public void Properties_CanBeModified()
    {
        // Arrange
        var settings = new ApiSettings();

        // Act
        settings.DevelopmentMode = true;
        settings.DetailedErrorMessages = true;

        // Assert
        settings.DevelopmentMode.Should().BeTrue();
        settings.DetailedErrorMessages.Should().BeTrue();
    }

    [Fact]
    public void DefaultDetailedErrorMessages_IsFalse_ForSecurity()
    {
        // Arrange & Act
        var settings = new ApiSettings();

        // Assert
        // In production, detailed errors should NOT be shown
        settings.DetailedErrorMessages.Should().BeFalse();
    }

    [Fact]
    public void DefaultDevelopmentMode_IsFalse_ForSecurity()
    {
        // Arrange & Act
        var settings = new ApiSettings();

        // Assert
        // By default, should run in non-development mode
        settings.DevelopmentMode.Should().BeFalse();
    }
}
