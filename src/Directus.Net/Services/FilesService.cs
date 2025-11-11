using Directus.Net.Abstractions;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Files service implementation
/// </summary>
public class FilesService : IFilesService
{
    private readonly IDirectusTransport _transport;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FilesService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesService"/> class
    /// </summary>
    public FilesService(IDirectusTransport transport, HttpClient httpClient, ILogger<FilesService>? logger = null)
    {
        _transport = transport;
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DirectusFile?> UploadFileAsync(string fileName, Stream fileStream, string? title = null, string? folder = null, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Uploading file: {FileName}", fileName);

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        
        content.Add(streamContent, "file", fileName);

        if (!string.IsNullOrEmpty(title))
        {
            content.Add(new StringContent(title), "title");
        }

        if (!string.IsNullOrEmpty(folder))
        {
            content.Add(new StringContent(folder), "folder");
        }

        return await _transport.SendMultipartAsync<DirectusFile>("/files", content, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DirectusFile?> GetFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Getting file metadata: {FileId}", fileId);

        return await _transport.GetAsync<DirectusFile>($"/files/{fileId}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream?> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Downloading file: {FileId}", fileId);

        var response = await _httpClient.GetAsync($"/assets/{fileId}", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Deleting file: {FileId}", fileId);

        await _transport.DeleteAsync($"/files/{fileId}", cancellationToken);
    }
}
