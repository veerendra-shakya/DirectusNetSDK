namespace Directus.Net.Abstractions;

/// <summary>
/// Service for file operations
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Uploads a file
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <param name="fileStream">The file stream</param>
    /// <param name="title">Optional file title</param>
    /// <param name="folder">Optional folder ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The uploaded file metadata</returns>
    Task<DirectusFile?> UploadFileAsync(string fileName, Stream fileStream, string? title = null, string? folder = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file metadata by ID
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The file metadata</returns>
    Task<DirectusFile?> GetFileAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The file stream</returns>
    Task<Stream?> DownloadFileAsync(string fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="fileId">The file ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a file in Directus
/// </summary>
public class DirectusFile
{
    /// <summary>
    /// Gets or sets the file ID
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the storage location
    /// </summary>
    public string? Storage { get; set; }

    /// <summary>
    /// Gets or sets the filename on disk
    /// </summary>
    public string? FilenameDisk { get; set; }

    /// <summary>
    /// Gets or sets the original filename
    /// </summary>
    public string? FilenameDownload { get; set; }

    /// <summary>
    /// Gets or sets the file title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the file type
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the folder ID
    /// </summary>
    public string? Folder { get; set; }

    /// <summary>
    /// Gets or sets the uploaded by user ID
    /// </summary>
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Gets or sets the upload timestamp
    /// </summary>
    public DateTime? UploadedOn { get; set; }

    /// <summary>
    /// Gets or sets the file size in bytes
    /// </summary>
    public long? Filesize { get; set; }

    /// <summary>
    /// Gets or sets the image width (for images)
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the image height (for images)
    /// </summary>
    public int? Height { get; set; }
}
