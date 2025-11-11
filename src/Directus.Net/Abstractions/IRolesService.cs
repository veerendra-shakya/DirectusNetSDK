using Directus.Net.Models;

namespace Directus.Net.Abstractions;

/// <summary>
/// Service for role management operations
/// </summary>
public interface IRolesService
{
    /// <summary>
    /// Gets all roles
    /// </summary>
    /// <param name="query">Optional query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection response with roles</returns>
    Task<CollectionResponse<DirectusRole>> GetRolesAsync(DirectusQuery? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by ID
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The role</returns>
    Task<DirectusRole?> GetRoleAsync(string roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new role
    /// </summary>
    /// <param name="role">The role to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created role</returns>
    Task<DirectusRole?> CreateRoleAsync(DirectusRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="role">The role data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated role</returns>
    Task<DirectusRole?> UpdateRoleAsync(string roleId, object role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role
    /// </summary>
    /// <param name="roleId">The role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteRoleAsync(string roleId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a role in Directus
/// </summary>
public class DirectusRole
{
    /// <summary>
    /// Gets or sets the role ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the role icon
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether IP access is enforced
    /// </summary>
    public bool? IpAccess { get; set; }

    /// <summary>
    /// Gets or sets whether enforcement of 2FA is required
    /// </summary>
    public bool? EnforceTfa { get; set; }

    /// <summary>
    /// Gets or sets whether this is an admin role
    /// </summary>
    public bool? AdminAccess { get; set; }

    /// <summary>
    /// Gets or sets whether app access is allowed
    /// </summary>
    public bool? AppAccess { get; set; }
}
