namespace Directus.Net.Exceptions;

/// <summary>
/// Exception thrown when authentication or authorization fails
/// </summary>
public class DirectusAuthException : DirectusException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusAuthException"/> class
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DirectusAuthException(string message) : base(message, 401, "UNAUTHORIZED")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusAuthException"/> class with an inner exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DirectusAuthException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
