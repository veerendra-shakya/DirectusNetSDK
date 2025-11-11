using Directus.Net.Abstractions;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Utils service implementation
/// </summary>
public class UtilsService : IUtilsService
{
    private readonly IDirectusTransport _transport;
    private readonly HttpClient _httpClient;
    private readonly ILogger<UtilsService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UtilsService"/> class
    /// </summary>
    public UtilsService(IDirectusTransport transport, HttpClient httpClient, ILogger<UtilsService>? logger = null)
    {
        _transport = transport;
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ServerInfo> GetServerInfoAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting server info");

        var response = await _transport.GetAsync<ServerInfo>("/server/info", cancellationToken);
        return response ?? new ServerInfo();
    }

    /// <inheritdoc />
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Performing health check");

        try
        {
            var response = await _httpClient.GetAsync("/server/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Health check failed");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<string?> GetOpenApiSpecAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting OpenAPI spec");

        var response = await _httpClient.GetAsync("/server/specs/oas", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
