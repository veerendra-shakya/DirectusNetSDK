using Directus.Net.Abstractions;
using Directus.Net.Models;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Roles service implementation
/// </summary>
public class RolesService : IRolesService
{
    private readonly IDirectusTransport _transport;
    private readonly ILogger<RolesService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RolesService"/> class
    /// </summary>
    public RolesService(IDirectusTransport transport, ILogger<RolesService>? logger = null)
    {
        _transport = transport;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CollectionResponse<DirectusRole>> GetRolesAsync(DirectusQuery? query = null, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting roles");

        var response = await _transport.GetAsync<CollectionResponse<DirectusRole>>("/roles", cancellationToken);
        return response ?? new CollectionResponse<DirectusRole> { Data = Array.Empty<DirectusRole>() };
    }

    /// <inheritdoc />
    public async Task<DirectusRole?> GetRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting role: {RoleId}", roleId);

        return await _transport.GetAsync<DirectusRole>($"/roles/{roleId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectusRole?> CreateRoleAsync(DirectusRole role, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Creating role: {Name}", role.Name);

        return await _transport.PostAsync<DirectusRole>("/roles", role, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectusRole?> UpdateRoleAsync(string roleId, object role, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Updating role: {RoleId}", roleId);

        return await _transport.PatchAsync<DirectusRole>($"/roles/{roleId}", role, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Deleting role: {RoleId}", roleId);

        await _transport.DeleteAsync($"/roles/{roleId}", cancellationToken);
    }
}
