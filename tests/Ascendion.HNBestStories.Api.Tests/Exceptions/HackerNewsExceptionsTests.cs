using Ascendion.HNBestStories.Api.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Exceptions;

/// <summary>
/// Unit tests for custom exception types.
/// Tests exception creation and inheritance.
/// </summary>
public class HackerNewsExceptionsTests
{
    [Fact]
    public void HackerNewsApiException_WithMessage_CreatesException()
    {
        // Arrange
        var message = "API error occurred";

        // Act
        var exception = new HackerNewsApiException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void HackerNewsApiException_WithMessageAndInnerException_CreatesException()
    {
        // Arrange
        var message = "API error occurred";
        var innerException = new HttpRequestException("Network error");

        // Act
        var exception = new HackerNewsApiException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void HackerNewsApiException_InheritsFromException()
    {
        // Arrange
        var exception = new HackerNewsApiException("Test");

        // Act & Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void HackerNewsApiException_CanBeCaught()
    {
        // Arrange & Act
        Exception? caughtException = null;
        try
        {
            throw new HackerNewsApiException("Test error");
        }
        catch (HackerNewsApiException ex)
        {
            caughtException = ex;
        }

        // Assert
        caughtException.Should().NotBeNull();
        caughtException.Should().BeOfType<HackerNewsApiException>();
    }

    [Fact]
    public void StoriesValidationException_WithMessage_CreatesException()
    {
        // Arrange
        var message = "Validation failed";

        // Act
        var exception = new StoriesValidationException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void StoriesValidationException_InheritsFromException()
    {
        // Arrange
        var exception = new StoriesValidationException("Test");

        // Act & Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void StoriesValidationException_CanBeCaught()
    {
        // Arrange & Act
        Exception? caughtException = null;
        try
        {
            throw new StoriesValidationException("Test validation error");
        }
        catch (StoriesValidationException ex)
        {
            caughtException = ex;
        }

        // Assert
        caughtException.Should().NotBeNull();
        caughtException.Should().BeOfType<StoriesValidationException>();
    }

    [Fact]
    public void HackerNewsApiException_CanBeCaughtAsException()
    {
        // Arrange & Act
        Exception? caughtException = null;
        try
        {
            throw new HackerNewsApiException("Test error");
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        // Assert
        caughtException.Should().NotBeNull();
    }

    [Fact]
    public void Exceptions_HaveCorrectTypeHierarchy()
    {
        // Arrange & Act
        var hackerNewsException = new HackerNewsApiException("Test");
        var validationException = new StoriesValidationException("Test");

        // Assert
        hackerNewsException.GetBaseException().Should().Be(hackerNewsException);
        validationException.GetBaseException().Should().Be(validationException);
    }
}
