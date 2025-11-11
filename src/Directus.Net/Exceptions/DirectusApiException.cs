namespace Directus.Net.Exceptions;

/// <summary>
/// Exception thrown when Directus API returns an error response
/// </summary>
public class DirectusApiException : DirectusException
{
    /// <summary>
    /// Gets the full error response from the API
    /// </summary>
    public object? ErrorResponse { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusApiException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="errorCode">The error code from Directus API</param>
    /// <param name="errorResponse">The full error response from the API</param>
    public DirectusApiException(string message, int statusCode, string? errorCode, object? errorResponse = null)
        : base(message, statusCode, errorCode)
    {
        ErrorResponse = errorResponse;
    }
}
