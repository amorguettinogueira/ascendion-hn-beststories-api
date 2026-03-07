namespace Ascendion.HNBestStories.Api.Exceptions;

/// <summary>
/// Represents an error that occurred while interacting with the Hacker News API.
/// Used to distinguish external API failures from internal application errors.
/// </summary>
public sealed class HackerNewsApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the HackerNewsApiException class.
    /// </summary>
    /// <param name="message">A message that describes the error.</param>
    public HackerNewsApiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the HackerNewsApiException class with a default error message.
    /// </summary>
    public HackerNewsApiException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the HackerNewsApiException class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public HackerNewsApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}