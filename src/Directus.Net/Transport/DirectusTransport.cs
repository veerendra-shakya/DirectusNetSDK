using System.Net.Http.Json;
using System.Text.Json;
using Directus.Net.Abstractions;
using Directus.Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Transport;

/// <summary>
/// HTTP transport implementation for Directus API
/// </summary>
public class DirectusTransport : IDirectusTransport
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<DirectusTransport>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectusTransport"/> class
    /// </summary>
    /// <param name="httpClient">The HTTP client</param>
    /// <param name="tokenStore">The token store</param>
    /// <param name="logger">Optional logger</param>
    public DirectusTransport(HttpClient httpClient, ITokenStore tokenStore, ILogger<DirectusTransport>? logger = null)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Get, path, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> PostAsync<T>(string path, object? body = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Post, path, body, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> PatchAsync<T>(string path, object? body = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(HttpMethod.Patch, path, body, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        await SendAsync<object>(HttpMethod.Delete, path, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> SendAsync<T>(HttpMethod method, string path, object? body = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(method, path);

        var token = await _tokenStore.GetAccessTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        if (body != null && (method == HttpMethod.Post || method == HttpMethod.Patch || method == HttpMethod.Put))
        {
            request.Content = JsonContent.Create(body, options: _jsonOptions);
        }

        _logger?.LogDebug("Sending {Method} request to {Path}", method, path);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, cancellationToken);
        }

        if (typeof(T) == typeof(object) || response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        var result = await response.Content.ReadFromJsonAsync<DirectusApiResponse<T>>(_jsonOptions, cancellationToken);
        return result != null ? result.Data : default;
    }

    /// <inheritdoc />
    public async Task<T?> SendMultipartAsync<T>(string path, MultipartFormDataContent content, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = content
        };

        var token = await _tokenStore.GetAccessTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        _logger?.LogDebug("Sending multipart POST request to {Path}", path);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response, cancellationToken);
        }

        var result = await response.Content.ReadFromJsonAsync<DirectusApiResponse<T>>(_jsonOptions, cancellationToken);
        return result != null ? result.Data : default;
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        string? errorMessage = null;
        string? errorCode = null;
        object? errorResponse = null;

        try
        {
            var errorContent = await response.Content.ReadFromJsonAsync<DirectusErrorResponse>(_jsonOptions, cancellationToken);
            if (errorContent?.Errors != null && errorContent.Errors.Length > 0)
            {
                errorMessage = errorContent.Errors[0].Message;
                errorCode = errorContent.Errors[0].Extensions?.Code;
                errorResponse = errorContent;
            }
        }
        catch
        {
            errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
        }

        errorMessage ??= $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";

        _logger?.LogError("API request failed: {StatusCode} - {Message}", response.StatusCode, errorMessage);

        throw new DirectusApiException(errorMessage, (int)response.StatusCode, errorCode, errorResponse);
    }
}

internal class DirectusApiResponse<T>
{
    public T? Data { get; set; }
}

internal class DirectusErrorResponse
{
    public DirectusError[]? Errors { get; set; }
}

internal class DirectusError
{
    public string? Message { get; set; }
    public DirectusErrorExtensions? Extensions { get; set; }
}

internal class DirectusErrorExtensions
{
    public string? Code { get; set; }
}
