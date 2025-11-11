namespace Directus.Net.Abstractions;

/// <summary>
/// Abstraction for storing and retrieving authentication tokens
/// </summary>
public interface ITokenStore
{
    /// <summary>
    /// Gets the access token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The access token, or null if not set</returns>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the access token
    /// </summary>
    /// <param name="token">The access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAccessTokenAsync(string? token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the refresh token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The refresh token, or null if not set</returns>
    Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the refresh token
    /// </summary>
    /// <param name="token">The refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetRefreshTokenAsync(string? token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all stored tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
