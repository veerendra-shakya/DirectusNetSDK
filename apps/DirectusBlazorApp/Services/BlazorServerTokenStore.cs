using Directus.Net.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DirectusBlazorApp.Services;

public class BlazorServerTokenStore : ITokenStore
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly ILogger<BlazorServerTokenStore> _logger;
    
    private const string AccessTokenKey = "directus_access_token";
    private const string RefreshTokenKey = "directus_refresh_token";

    public BlazorServerTokenStore(
        ProtectedSessionStorage sessionStorage,
        ILogger<BlazorServerTokenStore> logger)
    {
        _sessionStorage = sessionStorage;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(AccessTokenKey);
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access token");
            return null;
        }
    }

    public async Task SetAccessTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                await _sessionStorage.DeleteAsync(AccessTokenKey);
            }
            else
            {
                await _sessionStorage.SetAsync(AccessTokenKey, token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting access token");
        }
    }

    public async Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(RefreshTokenKey);
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token");
            return null;
        }
    }

    public async Task SetRefreshTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                await _sessionStorage.DeleteAsync(RefreshTokenKey);
            }
            else
            {
                await _sessionStorage.SetAsync(RefreshTokenKey, token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting refresh token");
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _sessionStorage.DeleteAsync(AccessTokenKey);
            await _sessionStorage.DeleteAsync(RefreshTokenKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing tokens");
        }
    }
}
