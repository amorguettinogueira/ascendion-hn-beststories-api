using Ascendion.HNBestStories.Api.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Exceptions;

/// <summary>
/// Unit tests for the StoriesValidationException.
/// Tests all three exception constructors.
/// </summary>
public class StoriesValidationExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_CreatesException()
    {
        // Arrange
        var message = "Validation failed: invalid input";

        // Act
        var exception = new StoriesValidationException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithoutArguments_CreatesException()
    {
        // Act
        var exception = new StoriesValidationException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeEmpty();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_CreatesException()
    {
        // Arrange
        var message = "Validation failed";
        var innerException = new ArgumentException("Invalid argument");

        // Act
        var exception = new StoriesValidationException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException!.Message.Should().Be("Invalid argument");
    }

    [Fact]
    public void Constructor_WithNullMessage_CreatesException()
    {
        // Act
        var exception = new StoriesValidationException(null, null);

        // Assert
        exception.Should().NotBeNull();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Exception_CanBeThrown_AndCaught()
    {
        // Act & Assert
        Action act = () => throw new StoriesValidationException("Number of stories must be positive");
        act.Should().Throw<StoriesValidationException>().WithMessage("Number of stories must be positive");
    }
}
