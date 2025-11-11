using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Directus.Net.Abstractions;
using Directus.Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// Realtime WebSocket service implementation
/// </summary>
public class RealtimeService : IRealtimeService
{
    private readonly string _baseUrl;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger<RealtimeService>? _logger;
    private ClientWebSocket? _webSocket;
    private readonly Dictionary<string, Action<object>> _subscriptions = new();
    private CancellationTokenSource? _receiveCts;
    private Task? _receiveTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealtimeService"/> class
    /// </summary>
    public RealtimeService(string baseUrl, ITokenStore tokenStore, ILogger<RealtimeService>? logger = null)
    {
        _baseUrl = baseUrl.Replace("http://", "ws://").Replace("https://", "wss://");
        _tokenStore = tokenStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsConnected => _webSocket?.State == WebSocketState.Open;

    /// <inheritdoc />
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            _logger?.LogDebug("WebSocket already connected");
            return;
        }

        _logger?.LogInformation("Connecting to WebSocket at {Url}", _baseUrl);

        _webSocket = new ClientWebSocket();
        
        var token = await _tokenStore.GetAccessTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
        }

        var wsUrl = $"{_baseUrl}/websocket";
        await _webSocket.ConnectAsync(new Uri(wsUrl), cancellationToken);

        _receiveCts = new CancellationTokenSource();
        _receiveTask = ReceiveMessagesAsync(_receiveCts.Token);

        _logger?.LogInformation("WebSocket connected");
    }

    /// <inheritdoc />
    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_webSocket?.State != WebSocketState.Open)
        {
            return;
        }

        _logger?.LogInformation("Disconnecting WebSocket");

        _receiveCts?.Cancel();

        if (_receiveTask != null)
        {
            try
            {
                await _receiveTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        if (_webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", cancellationToken);
        }

        _webSocket.Dispose();
        _webSocket = null;
        _receiveCts?.Dispose();
        _receiveCts = null;

        _logger?.LogInformation("WebSocket disconnected");
    }

    /// <inheritdoc />
    public async Task<string> SubscribeAsync<T>(string collection, Action<RealtimeMessage<T>> onMessage, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            await ConnectAsync(cancellationToken);
        }

        var subscriptionId = Guid.NewGuid().ToString();
        
        _logger?.LogDebug("Subscribing to collection {Collection} with ID {SubscriptionId}", collection, subscriptionId);

        _subscriptions[subscriptionId] = (data) =>
        {
            if (data is RealtimeMessage<T> message)
            {
                onMessage(message);
            }
        };

        var subscribeMessage = new
        {
            type = "subscribe",
            collection,
            uid = subscriptionId
        };

        await SendMessageAsync(subscribeMessage, cancellationToken);

        return subscriptionId;
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Unsubscribing from {SubscriptionId}", subscriptionId);

        if (_subscriptions.Remove(subscriptionId))
        {
            var unsubscribeMessage = new
            {
                type = "unsubscribe",
                uid = subscriptionId
            };

            await SendMessageAsync(unsubscribeMessage, cancellationToken);
        }
    }

    private async Task SendMessageAsync(object message, CancellationToken cancellationToken)
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            throw new DirectusException("WebSocket is not connected");
        }

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var bytes = Encoding.UTF8.GetBytes(json);
        await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }

    private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];

        try
        {
            while (_webSocket?.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger?.LogInformation("WebSocket closed by server");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger?.LogDebug("Received WebSocket message: {Message}", message);

                try
                {
                    ProcessMessage(message);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error processing WebSocket message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger?.LogDebug("WebSocket receive cancelled");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in WebSocket receive loop");
        }
    }

    private void ProcessMessage(string message)
    {
        using var doc = JsonDocument.Parse(message);
        var root = doc.RootElement;

        if (root.TryGetProperty("uid", out var uidElement))
        {
            var uid = uidElement.GetString();
            if (uid != null && _subscriptions.TryGetValue(uid, out var callback))
            {
                callback(message);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
