namespace Directus.Net.Abstractions;

/// <summary>
/// Service for utility operations
/// </summary>
public interface IUtilsService
{
    /// <summary>
    /// Gets server information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server information</returns>
    Task<ServerInfo> GetServerInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a health check
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health check status</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the OpenAPI specification
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>OpenAPI spec as JSON</returns>
    Task<string?> GetOpenApiSpecAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Server information
/// </summary>
public class ServerInfo
{
    /// <summary>
    /// Gets or sets the project information
    /// </summary>
    public ProjectInfo? Project { get; set; }
}

/// <summary>
/// Project information
/// </summary>
public class ProjectInfo
{
    /// <summary>
    /// Gets or sets the project name
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the project descriptor
    /// </summary>
    public string? ProjectDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the project logo
    /// </summary>
    public string? ProjectLogo { get; set; }

    /// <summary>
    /// Gets or sets the project color
    /// </summary>
    public string? ProjectColor { get; set; }

    /// <summary>
    /// Gets or sets whether public registration is allowed
    /// </summary>
    public bool? PublicRegistration { get; set; }
}
