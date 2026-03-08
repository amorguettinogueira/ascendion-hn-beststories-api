using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Services;
using Ascendion.HNBestStories.Api.Settings;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Services;

/// <summary>
/// Unit tests for the StoriesRequestValidator.
/// Tests input validation logic.
/// </summary>
public class StoriesRequestValidatorTests
{
    private readonly HackerNewsSettings _settings;
    private readonly IStoriesRequestValidator _validator;

    public StoriesRequestValidatorTests()
    {
        _settings = new HackerNewsSettings { MaxStoriesAllowed = 30 };
        _validator = new StoriesRequestValidator(_settings);
    }

    [Fact]
    public void Constructor_WithNullSettings_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new StoriesRequestValidator(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Validate_WithValidNumber_ReturnsNull()
    {
        // Arrange
        var numberOfStories = 10;

        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Validate_WithZero_ReturnsErrorMessage()
    {
        // Arrange
        var numberOfStories = 0;

        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("greater than 0");
    }

    [Fact]
    public void Validate_WithNegativeNumber_ReturnsErrorMessage()
    {
        // Arrange
        var numberOfStories = -5;

        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("greater than 0");
    }

    [Fact]
    public void Validate_WithNumberExceedingMax_ReturnsErrorMessage()
    {
        // Arrange
        var numberOfStories = 31; // Max is 30

        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("exceed");
        result.Should().Contain("30");
    }

    [Fact]
    public void Validate_WithMaxValue_ReturnsNull()
    {
        // Arrange
        var numberOfStories = 30; // Max allowed

        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Validate_WithOne_ReturnsNull()
    {
        // Arrange
        var numberOfStories = 1;

        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Validate_WithDifferentMaxSettings_RespectsMax()
    {
        // Arrange
        var settings = new HackerNewsSettings { MaxStoriesAllowed = 50 };
        var validator = new StoriesRequestValidator(settings);

        // Act
        var result1 = validator.Validate(40); // Within new max
        var result2 = validator.Validate(51); // Exceeds new max

        // Assert
        result1.Should().BeNull();
        result2.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Validate_MultipleCallsWithSameInput_ReturnConsistently()
    {
        // Arrange
        var numberOfStories = 15;

        // Act
        var result1 = _validator.Validate(numberOfStories);
        var result2 = _validator.Validate(numberOfStories);

        // Assert
        result1.Should().Be(result2);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(30)]
    public void Validate_WithValidRange_ReturnsNull(int numberOfStories)
    {
        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(31)]
    [InlineData(100)]
    public void Validate_WithInvalidRange_ReturnsErrorMessage(int numberOfStories)
    {
        // Act
        var result = _validator.Validate(numberOfStories);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }
}
