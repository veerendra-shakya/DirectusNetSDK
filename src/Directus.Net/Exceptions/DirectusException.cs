namespace Directus.Net.Exceptions;

/// <summary>
/// Base exception for all Directus SDK errors
/// </summary>
public class DirectusException : Exception
{
    /// <summary>
    /// Gets the HTTP status code if applicable
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Gets the error code from Directus API
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusException"/> class
    /// </summary>
    public DirectusException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DirectusException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusException"/> class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DirectusException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusException"/> class with specified error details
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="errorCode">The error code from Directus API</param>
    public DirectusException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
