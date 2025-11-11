namespace Directus.Net.Abstractions;

/// <summary>
/// Service for WebSocket realtime operations
/// </summary>
public interface IRealtimeService : IAsyncDisposable
{
    /// <summary>
    /// Connects to the WebSocket server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the WebSocket server
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to a collection for realtime updates
    /// </summary>
    /// <typeparam name="T">The item type</typeparam>
    /// <param name="collection">The collection name</param>
    /// <param name="onMessage">Callback for received messages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Subscription ID for unsubscribing</returns>
    Task<string> SubscribeAsync<T>(string collection, Action<RealtimeMessage<T>> onMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from a collection
    /// </summary>
    /// <param name="subscriptionId">The subscription ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the connection state
    /// </summary>
    bool IsConnected { get; }
}

/// <summary>
/// Realtime message from WebSocket
/// </summary>
/// <typeparam name="T">The data type</typeparam>
public class RealtimeMessage<T>
{
    /// <summary>
    /// Gets or sets the event type (create, update, delete)
    /// </summary>
    public string? Event { get; set; }

    /// <summary>
    /// Gets or sets the data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets the collection name
    /// </summary>
    public string? Collection { get; set; }
}
