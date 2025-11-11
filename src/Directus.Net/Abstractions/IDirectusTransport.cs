using System.Net.Http.Json;

namespace Directus.Net.Abstractions;

/// <summary>
/// Abstraction for HTTP transport to Directus API
/// </summary>
public interface IDirectusTransport
{
    /// <summary>
    /// Sends a GET request to the specified path
    /// </summary>
    /// <typeparam name="T">The type of the response</typeparam>
    /// <param name="path">The request path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized response</returns>
    Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a POST request to the specified path
    /// </summary>
    /// <typeparam name="T">The type of the response</typeparam>
    /// <param name="path">The request path</param>
    /// <param name="body">The request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized response</returns>
    Task<T?> PostAsync<T>(string path, object? body = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a PATCH request to the specified path
    /// </summary>
    /// <typeparam name="T">The type of the response</typeparam>
    /// <param name="path">The request path</param>
    /// <param name="body">The request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized response</returns>
    Task<T?> PatchAsync<T>(string path, object? body = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DELETE request to the specified path
    /// </summary>
    /// <param name="path">The request path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request with custom HTTP method
    /// </summary>
    /// <typeparam name="T">The type of the response</typeparam>
    /// <param name="method">The HTTP method</param>
    /// <param name="path">The request path</param>
    /// <param name="body">The request body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized response</returns>
    Task<T?> SendAsync<T>(HttpMethod method, string path, object? body = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a multipart form data request
    /// </summary>
    /// <typeparam name="T">The type of the response</typeparam>
    /// <param name="path">The request path</param>
    /// <param name="content">The multipart form data content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deserialized response</returns>
    Task<T?> SendMultipartAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default);
}
