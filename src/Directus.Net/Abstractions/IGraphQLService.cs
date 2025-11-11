namespace Directus.Net.Abstractions;

/// <summary>
/// Service for GraphQL operations
/// </summary>
public interface IGraphQLService
{
    /// <summary>
    /// Executes a GraphQL query
    /// </summary>
    /// <typeparam name="T">The response type</typeparam>
    /// <param name="query">The GraphQL query</param>
    /// <param name="variables">Optional variables</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query result</returns>
    Task<GraphQLResponse<T>> QueryAsync<T>(string query, object? variables = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a GraphQL mutation
    /// </summary>
    /// <typeparam name="T">The response type</typeparam>
    /// <param name="mutation">The GraphQL mutation</param>
    /// <param name="variables">Optional variables</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The mutation result</returns>
    Task<GraphQLResponse<T>> MutateAsync<T>(string mutation, object? variables = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// GraphQL response wrapper
/// </summary>
/// <typeparam name="T">The data type</typeparam>
public class GraphQLResponse<T>
{
    /// <summary>
    /// Gets or sets the response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets any errors
    /// </summary>
    public GraphQLError[]? Errors { get; set; }
}

/// <summary>
/// GraphQL error
/// </summary>
public class GraphQLError
{
    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the error locations
    /// </summary>
    public object? Locations { get; set; }

    /// <summary>
    /// Gets or sets the error path
    /// </summary>
    public object[]? Path { get; set; }

    /// <summary>
    /// Gets or sets additional extensions
    /// </summary>
    public object? Extensions { get; set; }
}
