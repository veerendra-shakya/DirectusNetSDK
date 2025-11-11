namespace Directus.Net.Models;

/// <summary>
/// Response wrapper for collection queries
/// </summary>
/// <typeparam name="T">The item type</typeparam>
public class CollectionResponse<T>
{
    /// <summary>
    /// Gets or sets the data items
    /// </summary>
    public T[]? Data { get; set; }

    /// <summary>
    /// Gets or sets the metadata
    /// </summary>
    public ResponseMeta? Meta { get; set; }
}

/// <summary>
/// Response metadata
/// </summary>
public class ResponseMeta
{
    /// <summary>
    /// Gets or sets the total count of items
    /// </summary>
    public int? TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the filter count
    /// </summary>
    public int? FilterCount { get; set; }
}
