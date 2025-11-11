namespace Directus.Net.Abstractions;

/// <summary>
/// Service for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the access token using the refresh token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication response with tokens</returns>
    Task<AuthResponse> RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the current user and clears tokens
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current access token
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current access token, or null if not authenticated</returns>
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a static access token
    /// </summary>
    /// <param name="token">The access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetTokenAsync(string token, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authentication response from Directus API
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time in milliseconds
    /// </summary>
    public long? Expires { get; set; }
}
