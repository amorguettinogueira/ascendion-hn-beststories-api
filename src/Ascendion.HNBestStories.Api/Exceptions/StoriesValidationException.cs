namespace Ascendion.HNBestStories.Api.Exceptions;

/// <summary>
/// Represents a validation error in the request for best stories.
/// Distinguishes from other error types for better error handling.
/// </summary>
public sealed class StoriesValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the StoriesValidationException class.
    /// </summary>
    /// <param name="message">A message that describes the validation error.</param>
    public StoriesValidationException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the HackerNewsApiException class with a default error message.
    /// </summary>
    public StoriesValidationException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the StoriesValidationException class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public StoriesValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}