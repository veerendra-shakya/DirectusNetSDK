namespace Directus.Net.Abstractions;

/// <summary>
/// Main Directus client interface
/// </summary>
public interface IDirectusClient
{
    /// <summary>
    /// Gets the authentication service
    /// </summary>
    IAuthService Auth { get; }

    /// <summary>
    /// Gets the items service
    /// </summary>
    IItemsService Items { get; }

    /// <summary>
    /// Gets the files service
    /// </summary>
    IFilesService Files { get; }

    /// <summary>
    /// Gets the users service
    /// </summary>
    IUsersService Users { get; }

    /// <summary>
    /// Gets the roles service
    /// </summary>
    IRolesService Roles { get; }

    /// <summary>
    /// Gets the GraphQL service
    /// </summary>
    IGraphQLService GraphQL { get; }

    /// <summary>
    /// Gets the realtime service
    /// </summary>
    IRealtimeService Realtime { get; }

    /// <summary>
    /// Gets the utils service
    /// </summary>
    IUtilsService Utils { get; }

    /// <summary>
    /// Gets the underlying HTTP transport
    /// </summary>
    IDirectusTransport Transport { get; }

    /// <summary>
    /// Gets the base URL of the Directus instance
    /// </summary>
    string BaseUrl { get; }
}
