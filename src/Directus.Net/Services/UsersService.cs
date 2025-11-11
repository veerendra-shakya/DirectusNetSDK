using Directus.Net.Abstractions;
using Directus.Net.Models;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Users service implementation
/// </summary>
public class UsersService : IUsersService
{
    private readonly IDirectusTransport _transport;
    private readonly ILogger<UsersService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersService"/> class
    /// </summary>
    public UsersService(IDirectusTransport transport, ILogger<UsersService>? logger = null)
    {
        _transport = transport;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CollectionResponse<DirectusUser>> GetUsersAsync(DirectusQuery? query = null, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting users");

        var response = await _transport.GetAsync<CollectionResponse<DirectusUser>>("/users", cancellationToken);
        return response ?? new CollectionResponse<DirectusUser> { Data = Array.Empty<DirectusUser>() };
    }

    /// <inheritdoc />
    public async Task<DirectusUser?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting user: {UserId}", userId);

        return await _transport.GetAsync<DirectusUser>($"/users/{userId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectusUser?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting current user");

        return await _transport.GetAsync<DirectusUser>("/users/me", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectusUser?> CreateUserAsync(DirectusUser user, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Creating user: {Email}", user.Email);

        return await _transport.PostAsync<DirectusUser>("/users", user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectusUser?> UpdateUserAsync(string userId, object user, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Updating user: {UserId}", userId);

        return await _transport.PatchAsync<DirectusUser>($"/users/{userId}", user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Deleting user: {UserId}", userId);

        await _transport.DeleteAsync($"/users/{userId}", cancellationToken);
    }
}
