using Directus.Net.Abstractions;
using Directus.Net.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register Directus services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Directus client services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="baseUrl">The base URL of the Directus instance</param>
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddDirectus(
        this IServiceCollection services,
        string baseUrl,
        Action<DirectusClientOptions>? configure = null)
    {
        var options = new DirectusClientOptions();
        configure?.Invoke(options);

        services.AddSingleton<ITokenStore>(options.TokenStore ?? new InMemoryTokenStore());

        services.AddHttpClient<IDirectusClient, DirectusClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
            if (options.Timeout.HasValue)
            {
                client.Timeout = options.Timeout.Value;
            }
        });

        services.AddSingleton<IDirectusClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(DirectusClient));
            var loggerFactory = sp.GetService<ILoggerFactory>();

            return new DirectusClient(baseUrl, new DirectusClientOptions
            {
                HttpClient = httpClient,
                TokenStore = sp.GetRequiredService<ITokenStore>(),
                Timeout = options.Timeout,
                AutoRefreshToken = options.AutoRefreshToken,
                RetryPolicy = options.RetryPolicy
            }, loggerFactory);
        });

        return services;
    }
}
