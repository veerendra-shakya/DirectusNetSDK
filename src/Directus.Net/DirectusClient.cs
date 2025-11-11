using Directus.Net.Abstractions;
using Directus.Net.Services;
using Directus.Net.Storage;
using Directus.Net.Transport;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Directus.Net;

/// <summary>
/// Main Directus client for interacting with Directus API
/// </summary>
public class DirectusClient : IDirectusClient
{
    /// <inheritdoc />
    public IAuthService Auth { get; }

    /// <inheritdoc />
    public IItemsService Items { get; }

    /// <inheritdoc />
    public IFilesService Files { get; }

    /// <inheritdoc />
    public IUsersService Users { get; }

    /// <inheritdoc />
    public IRolesService Roles { get; }

    /// <inheritdoc />
    public IGraphQLService GraphQL { get; }

    /// <inheritdoc />
    public IRealtimeService Realtime { get; }

    /// <inheritdoc />
    public IUtilsService Utils { get; }

    /// <inheritdoc />
    public IDirectusTransport Transport { get; }

    /// <inheritdoc />
    public string BaseUrl { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusClient"/> class
    /// </summary>
    /// <param name="baseUrl">The base URL of the Directus instance</param>
    /// <param name="options">Optional configuration options</param>
    /// <param name="loggerFactory">Optional logger factory</param>
    public DirectusClient(string baseUrl, DirectusClientOptions? options = null, ILoggerFactory? loggerFactory = null)
    {
        BaseUrl = baseUrl.TrimEnd('/');
        options ??= new DirectusClientOptions();

        var tokenStore = options.TokenStore ?? new InMemoryTokenStore();

        var httpClient = CreateHttpClient(options);

        Transport = new DirectusTransport(
            httpClient,
            tokenStore,
            loggerFactory?.CreateLogger<DirectusTransport>()
        );

        Auth = new AuthService(
            Transport,
            tokenStore,
            loggerFactory?.CreateLogger<AuthService>()
        );

        Items = new ItemsService(
            Transport,
            loggerFactory?.CreateLogger<ItemsService>()
        );

        Files = new FilesService(
            Transport,
            httpClient,
            loggerFactory?.CreateLogger<FilesService>()
        );

        Users = new UsersService(
            Transport,
            loggerFactory?.CreateLogger<UsersService>()
        );

        Roles = new RolesService(
            Transport,
            loggerFactory?.CreateLogger<RolesService>()
        );

        GraphQL = new GraphQLService(
            Transport,
            loggerFactory?.CreateLogger<GraphQLService>()
        );

        Realtime = new RealtimeService(
            BaseUrl,
            tokenStore,
            loggerFactory?.CreateLogger<RealtimeService>()
        );

        Utils = new UtilsService(
            Transport,
            httpClient,
            loggerFactory?.CreateLogger<UtilsService>()
        );
    }

    private HttpClient CreateHttpClient(DirectusClientOptions options)
    {
        var httpClient = options.HttpClient ?? new HttpClient();

        if (httpClient.BaseAddress == null)
        {
            httpClient.BaseAddress = new Uri(BaseUrl);
        }

        if (options.Timeout.HasValue)
        {
            httpClient.Timeout = options.Timeout.Value;
        }

        return httpClient;
    }
}

/// <summary>
/// Configuration options for DirectusClient
/// </summary>
public class DirectusClientOptions
{
    /// <summary>
    /// Gets or sets the HTTP client to use
    /// </summary>
    public HttpClient? HttpClient { get; set; }

    /// <summary>
    /// Gets or sets the token store implementation
    /// </summary>
    public ITokenStore? TokenStore { get; set; }

    /// <summary>
    /// Gets or sets the HTTP request timeout
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Gets or sets whether to enable automatic token refresh
    /// </summary>
    public bool AutoRefreshToken { get; set; } = true;

    /// <summary>
    /// Gets or sets the retry policy
    /// </summary>
    public ResiliencePipeline? RetryPolicy { get; set; }
}
