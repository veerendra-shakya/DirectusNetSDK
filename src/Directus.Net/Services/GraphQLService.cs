using Directus.Net.Abstractions;
using Directus.Net.Exceptions;
using Microsoft.Extensions.Logging;

namespace Directus.Net.Services;

/// <summary>
/// GraphQL service implementation
/// </summary>
public class GraphQLService : IGraphQLService
{
    private readonly IDirectusTransport _transport;
    private readonly ILogger<GraphQLService>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphQLService"/> class
    /// </summary>
    public GraphQLService(IDirectusTransport transport, ILogger<GraphQLService>? logger = null)
    {
        _transport = transport;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<GraphQLResponse<T>> QueryAsync<T>(string query, object? variables = null, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Executing GraphQL query");

        return await ExecuteGraphQLAsync<T>(query, variables, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<GraphQLResponse<T>> MutateAsync<T>(string mutation, object? variables = null, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Executing GraphQL mutation");

        return await ExecuteGraphQLAsync<T>(mutation, variables, cancellationToken);
    }

    private async Task<GraphQLResponse<T>> ExecuteGraphQLAsync<T>(string query, object? variables, CancellationToken cancellationToken)
    {
        var request = new
        {
            query,
            variables
        };

        var response = await _transport.PostAsync<GraphQLResponse<T>>("/graphql", request, cancellationToken);

        if (response == null)
        {
            throw new DirectusException("GraphQL request failed: No response from server");
        }

        if (response.Errors != null && response.Errors.Length > 0)
        {
            var errorMessage = string.Join(", ", response.Errors.Select(e => e.Message));
            _logger?.LogError("GraphQL errors: {Errors}", errorMessage);
        }

        return response;
    }
}
