using System.Text;
using Directus.Net.Abstractions;
using Directus.Net.Models;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Items service implementation
/// </summary>
public class ItemsService : IItemsService
{
    private readonly IDirectusTransport _transport;
    private readonly ILogger<ItemsService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsService"/> class
    /// </summary>
    public ItemsService(IDirectusTransport transport, ILogger<ItemsService>? logger = null)
    {
        _transport = transport;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CollectionResponse<T>> ReadItemsAsync<T>(string collection, DirectusQuery? query = null, CancellationToken cancellationToken = default) where T : class
    {
        var path = BuildPath($"/items/{collection}", query);
        _logger?.LogDebug("Reading items from collection: {Collection}", collection);

        var response = await _transport.GetAsync<CollectionResponse<T>>(path, cancellationToken);
        return response ?? new CollectionResponse<T> { Data = Array.Empty<T>() };
    }

    /// <inheritdoc />
    public async Task<T?> ReadItemAsync<T>(string collection, string id, DirectusQuery? query = null, CancellationToken cancellationToken = default) where T : class
    {
        var path = BuildPath($"/items/{collection}/{id}", query);
        _logger?.LogDebug("Reading item {Id} from collection: {Collection}", id, collection);

        return await _transport.GetAsync<T>(path, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> CreateItemAsync<T>(string collection, T item, CancellationToken cancellationToken = default) where T : class
    {
        _logger?.LogDebug("Creating item in collection: {Collection}", collection);

        return await _transport.PostAsync<T>($"/items/{collection}", item, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T[]?> CreateItemsAsync<T>(string collection, T[] items, CancellationToken cancellationToken = default) where T : class
    {
        _logger?.LogDebug("Creating {Count} items in collection: {Collection}", items.Length, collection);

        return await _transport.PostAsync<T[]>($"/items/{collection}", items, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> UpdateItemAsync<T>(string collection, string id, object item, CancellationToken cancellationToken = default) where T : class
    {
        _logger?.LogDebug("Updating item {Id} in collection: {Collection}", id, collection);

        return await _transport.PatchAsync<T>($"/items/{collection}/{id}", item, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T[]?> UpdateItemsAsync<T>(string collection, string[] ids, object item, CancellationToken cancellationToken = default) where T : class
    {
        _logger?.LogDebug("Updating {Count} items in collection: {Collection}", ids.Length, collection);

        return await _transport.PatchAsync<T[]>($"/items/{collection}", new
        {
            keys = ids,
            data = item
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteItemAsync(string collection, string id, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Deleting item {Id} from collection: {Collection}", id, collection);

        await _transport.DeleteAsync($"/items/{collection}/{id}", cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteItemsAsync(string collection, string[] ids, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Deleting {Count} items from collection: {Collection}", ids.Length, collection);

        await _transport.DeleteAsync($"/items/{collection}/{string.Join(",", ids)}", cancellationToken);
    }

    private static string BuildPath(string basePath, DirectusQuery? query)
    {
        if (query == null)
        {
            return basePath;
        }

        var queryParams = new List<string>();

        if (query.Filter != null)
        {
            queryParams.Add($"filter={Uri.EscapeDataString(System.Text.Json.JsonSerializer.Serialize(query.Filter))}");
        }

        if (query.Sort != null && query.Sort.Length > 0)
        {
            queryParams.Add($"sort={string.Join(",", query.Sort)}");
        }

        if (query.Limit.HasValue)
        {
            queryParams.Add($"limit={query.Limit.Value}");
        }

        if (query.Offset.HasValue)
        {
            queryParams.Add($"offset={query.Offset.Value}");
        }

        if (query.Page.HasValue)
        {
            queryParams.Add($"page={query.Page.Value}");
        }

        if (query.Fields != null && query.Fields.Length > 0)
        {
            queryParams.Add($"fields={string.Join(",", query.Fields)}");
        }

        if (!string.IsNullOrEmpty(query.Search))
        {
            queryParams.Add($"search={Uri.EscapeDataString(query.Search)}");
        }

        if (!string.IsNullOrEmpty(query.Meta))
        {
            queryParams.Add($"meta={query.Meta}");
        }

        if (query.GroupBy != null && query.GroupBy.Length > 0)
        {
            queryParams.Add($"groupBy={string.Join(",", query.GroupBy)}");
        }

        if (queryParams.Count == 0)
        {
            return basePath;
        }

        return $"{basePath}?{string.Join("&", queryParams)}";
    }
}
