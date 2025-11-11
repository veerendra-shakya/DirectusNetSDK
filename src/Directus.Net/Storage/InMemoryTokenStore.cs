using Directus.Net.Abstractions;

namespace Directus.Net.Storage;

/// <summary>
/// In-memory implementation of token storage
/// </summary>
public class InMemoryTokenStore : ITokenStore
{
    private string? _accessToken;
    private string? _refreshToken;

    /// <inheritdoc />
    public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_accessToken);
    }

    /// <inheritdoc />
    public Task SetAccessTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        _accessToken = token;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_refreshToken);
    }

    /// <inheritdoc />
    public Task SetRefreshTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        _refreshToken = token;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        _accessToken = null;
        _refreshToken = null;
        return Task.CompletedTask;
    }
}
