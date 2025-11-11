using System.Text.Json;
using Directus.Net.Abstractions;
using Directus.Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IDirectusTransport _transport;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<AuthService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class
    /// </summary>
    public AuthService(IDirectusTransport transport, ITokenStore tokenStore, ILogger<AuthService>? logger = null)
    {
        _transport = transport;
        _tokenStore = tokenStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Attempting login for user: {Email}", email);

        var response = await _transport.PostAsync<AuthData>("/auth/login", new
        {
            email,
            password
        }, cancellationToken);

        if (response == null)
        {
            throw new DirectusAuthException("Login failed: No response from server");
        }

        var authResponse = new AuthResponse
        {
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken,
            Expires = response.Expires
        };

        if (!string.IsNullOrEmpty(authResponse.AccessToken))
        {
            await _tokenStore.SetAccessTokenAsync(authResponse.AccessToken, cancellationToken);
        }

        if (!string.IsNullOrEmpty(authResponse.RefreshToken))
        {
            await _tokenStore.SetRefreshTokenAsync(authResponse.RefreshToken, cancellationToken);
        }

        _logger?.LogInformation("Login successful for user: {Email}", email);

        return authResponse;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RefreshAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Refreshing access token");

        var refreshToken = await _tokenStore.GetRefreshTokenAsync(cancellationToken);
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new DirectusAuthException("No refresh token available");
        }

        var response = await _transport.PostAsync<AuthData>("/auth/refresh", new
        {
            refresh_token = refreshToken
        }, cancellationToken);

        if (response == null)
        {
            throw new DirectusAuthException("Token refresh failed: No response from server");
        }

        var authResponse = new AuthResponse
        {
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken,
            Expires = response.Expires
        };

        if (!string.IsNullOrEmpty(authResponse.AccessToken))
        {
            await _tokenStore.SetAccessTokenAsync(authResponse.AccessToken, cancellationToken);
        }

        if (!string.IsNullOrEmpty(authResponse.RefreshToken))
        {
            await _tokenStore.SetRefreshTokenAsync(authResponse.RefreshToken, cancellationToken);
        }

        _logger?.LogInformation("Token refresh successful");

        return authResponse;
    }

    /// <inheritdoc />
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("Logging out");

        var refreshToken = await _tokenStore.GetRefreshTokenAsync(cancellationToken);
        
        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                await _transport.PostAsync<object>("/auth/logout", new
                {
                    refresh_token = refreshToken
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Logout request failed, clearing tokens anyway");
            }
        }

        await _tokenStore.ClearAsync(cancellationToken);
        _logger?.LogInformation("Logout successful");
    }

    /// <inheritdoc />
    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        return await _tokenStore.GetAccessTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await _tokenStore.SetAccessTokenAsync(token, cancellationToken);
    }
}

internal class AuthData
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public long? Expires { get; set; }
}
