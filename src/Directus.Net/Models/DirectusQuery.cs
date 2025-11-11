namespace Directus.Net.Models;

/// <summary>
/// Query parameters for Directus API requests
/// </summary>
public class DirectusQuery
{
    /// <summary>
    /// Gets or sets the filter criteria
    /// </summary>
    public object? Filter { get; set; }

    /// <summary>
    /// Gets or sets the sort order (comma-separated field names, prefix with - for descending)
    /// </summary>
    public string[]? Sort { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of items to return
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the number of items to skip
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// Gets or sets the page number (alternative to offset)
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// Gets or sets the fields to include in the response
    /// </summary>
    public string[]? Fields { get; set; }

    /// <summary>
    /// Gets or sets the search query
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Gets or sets the metadata options (total_count, filter_count)
    /// </summary>
    public string? Meta { get; set; }

    /// <summary>
    /// Gets or sets deep query parameters for nested relations
    /// </summary>
    public Dictionary<string, object>? Deep { get; set; }

    /// <summary>
    /// Gets or sets the aggregation functions
    /// </summary>
    public Dictionary<string, string>? Aggregate { get; set; }

    /// <summary>
    /// Gets or sets the groupBy fields
    /// </summary>
    public string[]? GroupBy { get; set; }
}
