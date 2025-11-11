using Directus.Net.Models;

namespace Directus.Net.Abstractions;

/// <summary>
/// Service for CRUD operations on collection items
/// </summary>
public interface IItemsService
{
    /// <summary>
    /// Reads multiple items from a collection
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="query">Optional query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection response with items</returns>
    Task<CollectionResponse<T>> ReadItemsAsync<T>(string collection, DirectusQuery? query = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Reads a single item by ID
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="id">The item ID</param>
    /// <param name="query">Optional query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The item</returns>
    Task<T?> ReadItemAsync<T>(string collection, string id, DirectusQuery? query = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Creates a single item
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="item">The item to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created item</returns>
    Task<T?> CreateItemAsync<T>(string collection, T item, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Creates multiple items
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="items">The items to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created items</returns>
    Task<T[]?> CreateItemsAsync<T>(string collection, T[] items, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Updates a single item by ID
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="id">The item ID</param>
    /// <param name="item">The item data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated item</returns>
    Task<T?> UpdateItemAsync<T>(string collection, string id, object item, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Updates multiple items by IDs
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="ids">The item IDs</param>
    /// <param name="item">The item data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated items</returns>
    Task<T[]?> UpdateItemsAsync<T>(string collection, string[] ids, object item, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Deletes a single item by ID
    /// </summary>
    /// <param name="collection">The collection name</param>
    /// <param name="id">The item ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteItemAsync(string collection, string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple items by IDs
    /// </summary>
    /// <param name="collection">The collection name</param>
    /// <param name="ids">The item IDs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteItemsAsync(string collection, string[] ids, CancellationToken cancellationToken = default);
}
