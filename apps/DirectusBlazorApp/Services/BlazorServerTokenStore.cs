using Directus.Net.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DirectusBlazorApp.Services;

public class BlazorServerTokenStore : ITokenStore
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private const string AccessTokenKey = "directus_access_token";
    private const string RefreshTokenKey = "directus_refresh_token";

    public BlazorServerTokenStore(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(AccessTokenKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(RefreshTokenKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAccessTokenAsync(string? token, CancellationToken cancellationToken = default)
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

    public async Task SetRefreshTokenAsync(string? token, CancellationToken cancellationToken = default)
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

    public async Task ClearTokensAsync(CancellationToken cancellationToken = default)
    {
        await _sessionStorage.DeleteAsync(AccessTokenKey);
        await _sessionStorage.DeleteAsync(RefreshTokenKey);
    }
}