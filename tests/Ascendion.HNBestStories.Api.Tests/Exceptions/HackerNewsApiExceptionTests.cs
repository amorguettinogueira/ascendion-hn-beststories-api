using Ascendion.HNBestStories.Api.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Exceptions;

/// <summary>
/// Unit tests for the HackerNewsApiException.
/// Tests all exception constructors.
/// </summary>
public class HackerNewsApiExceptionTests
{
    [Fact]
    public void Constructor_WithoutArguments_CreatesException()
    {
        // Act
        var exception = new HackerNewsApiException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeEmpty();
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessage_CreatesException()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var exception = new HackerNewsApiException(message);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_CreatesException()
    {
        // Arrange
        var message = "Outer error";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new HackerNewsApiException(message, innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException!.Message.Should().Be("Inner error");
    }

    [Fact]
    public void Exception_CanBeThrown_AndCaught()
    {
        // Act & Assert
        Action act = () => throw new HackerNewsApiException("Test exception");
        act.Should().Throw<HackerNewsApiException>().WithMessage("Test exception");
    }
}
