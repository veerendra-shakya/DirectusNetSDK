using Directus.Net.Models;

namespace Directus.Net.Abstractions;

/// <summary>
/// Service for user management operations
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Gets all users
    /// </summary>
    /// <param name="query">Optional query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection response with users</returns>
    Task<CollectionResponse<DirectusUser>> GetUsersAsync(DirectusQuery? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user</returns>
    Task<DirectusUser?> GetUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the currently authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current user</returns>
    Task<DirectusUser?> GetMeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user</returns>
    Task<DirectusUser?> CreateUserAsync(DirectusUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="user">The user data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user</returns>
    Task<DirectusUser?> UpdateUserAsync(string userId, object user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a user in Directus
/// </summary>
public class DirectusUser
{
    /// <summary>
    /// Gets or sets the user ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the first name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the password (write-only)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the role ID
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the user status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the avatar file ID
    /// </summary>
    public string? Avatar { get; set; }
}
