# Directus.Net

A comprehensive, production-ready .NET 9 SDK for [Directus](https://directus.io), mirroring the official JavaScript SDK (@directus/sdk) with full support for REST, GraphQL, and WebSocket APIs.

## Features

- **Full REST API Support** - Complete CRUD operations on items, files, users, roles, and utilities
- **Advanced Query Builder** - Fluent API with filters, sorting, pagination, deep relations, and aggregations
- **GraphQL Support** - Execute queries and mutations with typed responses
- **WebSocket Realtime** - Subscribe to collection updates with automatic reconnection
- **Type-Safe & Async** - Fully asynchronous with comprehensive XML documentation
- **Token Authentication** - Login, logout, automatic refresh, and extensible token storage
- **Resilience Policies** - Polly-based retry and circuit breaker patterns
- **Dependency Injection** - Native support for Microsoft.Extensions.DependencyInjection
- **Extensible Architecture** - Clean abstractions for custom implementations
- **Blazor Server Integration** - Full-featured Blazor Server demo application included

## Installation

```bash
dotnet add package Directus.Net
```

## Quick Start

### Basic Usage

```csharp
using Directus.Net;

// Create client
var client = new DirectusClient("https://your-directus-instance.com");

// Login
await client.Auth.LoginAsync("user@example.com", "password");

// Query items
var posts = await client.Items.ReadItemsAsync<Post>("posts", 
    new QueryBuilder()
        .Limit(10)
        .Sort("-date_created")
        .WithFilter(new { status = Filter.Eq("published") })
        .Build());

// Create item
var newPost = await client.Items.CreateItemAsync("posts", new Post
{
    Title = "Hello Directus!",
    Content = "My first post",
    Status = "draft"
});

// Logout
await client.Auth.LogoutAsync();
```

### Dependency Injection

```csharp
using Directus.Net.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDirectus("https://your-directus-instance.com", options =>
{
    options.Timeout = TimeSpan.FromSeconds(30);
});

var serviceProvider = services.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IDirectusClient>();
```

## Advanced Usage

### Query Builder with Filters

```csharp
using Directus.Net.Models;

var query = new QueryBuilder()
    .WithFilter(Filter.And(
        new { status = Filter.Eq("published") },
        new { views = Filter.Gt(100) },
        new { title = Filter.IContains("directus") }
    ))
    .Sort("-date_created", "title")
    .Limit(20)
    .Offset(0)
    .Fields("id", "title", "date_created", "author.name")
    .WithMeta("total_count")
    .Build();

var result = await client.Items.ReadItemsAsync<Article>("articles", query);
Console.WriteLine($"Total articles: {result.Meta?.TotalCount}");
```

### GraphQL

```csharp
var query = @"
    query GetPosts($limit: Int!) {
        posts(limit: $limit, filter: { status: { _eq: ""published"" } }) {
            id
            title
            author {
                name
                email
            }
        }
    }";

var response = await client.GraphQL.QueryAsync<PostsResponse>(query, new { limit = 10 });
```

### WebSocket Realtime

```csharp
await client.Realtime.ConnectAsync();

var subscriptionId = await client.Realtime.SubscribeAsync<Post>("posts", message =>
{
    Console.WriteLine($"Event: {message.Event}");
    Console.WriteLine($"Post: {message.Data?.Title}");
});

// Later: unsubscribe
await client.Realtime.UnsubscribeAsync(subscriptionId);
await client.Realtime.DisconnectAsync();
```

### File Upload

```csharp
using var fileStream = File.OpenRead("image.jpg");

var uploadedFile = await client.Files.UploadFileAsync(
    "image.jpg", 
    fileStream,
    title: "My Image",
    folder: "photos");

Console.WriteLine($"Uploaded: {uploadedFile?.Id}");
```

### User Management

```csharp
// Get current user
var currentUser = await client.Users.GetMeAsync();

// Get all users
var users = await client.Users.GetUsersAsync();

// Create user
var newUser = await client.Users.CreateUserAsync(new DirectusUser
{
    Email = "newuser@example.com",
    Password = "secure-password",
    FirstName = "John",
    LastName = "Doe",
    Role = "editor-role-id"
});
```

## Filter Operators

The SDK supports all Directus filter operators:

```csharp
// Comparison
Filter.Eq(value)           // Equals
Filter.Neq(value)          // Not equals
Filter.Lt(value)           // Less than
Filter.Lte(value)          // Less than or equal
Filter.Gt(value)           // Greater than
Filter.Gte(value)          // Greater than or equal
Filter.In(values)          // In array
Filter.Nin(values)         // Not in array

// String
Filter.Contains(value)     // Contains (case-sensitive)
Filter.IContains(value)    // Contains (case-insensitive)
Filter.StartsWith(value)   // Starts with
Filter.EndsWith(value)     // Ends with

// Logical
Filter.And(conditions)     // AND
Filter.Or(conditions)      // OR
Filter.IsNull()            // Is null
Filter.IsNotNull()         // Is not null
Filter.IsEmpty()           // Is empty
Filter.IsNotEmpty()        // Is not empty
```

## Configuration Options

```csharp
var client = new DirectusClient("https://directus.example.com", new DirectusClientOptions
{
    Timeout = TimeSpan.FromSeconds(30),
    AutoRefreshToken = true,
    TokenStore = new CustomTokenStore(), // Implement ITokenStore
    HttpClient = customHttpClient
});
```

## Custom Token Storage

Implement `ITokenStore` for custom token persistence:

```csharp
public class DatabaseTokenStore : ITokenStore
{
    public async Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        // Load from database
    }

    public async Task SetAccessTokenAsync(string? token, CancellationToken ct = default)
    {
        // Save to database
    }

    // Implement other methods...
}
```

## Architecture

- **IDirectusClient** - Main client interface with all services
- **IDirectusTransport** - HTTP transport abstraction
- **ITokenStore** - Token storage abstraction
- **IAuthService** - Authentication operations
- **IItemsService** - CRUD operations on collections
- **IFilesService** - File upload/download
- **IUsersService** - User management
- **IRolesService** - Role management
- **IGraphQLService** - GraphQL queries/mutations
- **IRealtimeService** - WebSocket subscriptions
- **IUtilsService** - Server utilities

## Requirements

- .NET 9.0 or later
- Directus 11.1.0+ (recommended)

## Sample Applications

### Console Application
See `samples/Directus.Net.Sample` for a complete console application demonstrating:
- Authentication
- Health checks
- Query building
- Filter examples
- Server information retrieval

### Blazor Server Application
See `apps/DirectusBlazorApp` for a full-featured Blazor Server application demonstrating:
- **Directus Instance**: https://data.gwaliorsmartcity.org
- Secure authentication with protected session storage
- User dashboard with profile information
- Dynamic collection browsing
- Server health monitoring
- Login/logout flows
- Integration patterns for Blazor applications

Run the Blazor app:
```bash
dotnet run --project apps/DirectusBlazorApp/DirectusBlazorApp.csproj
# Navigate to http://localhost:5000
```

The Blazor app showcases:
- Custom `BlazorServerTokenStore` for secure token management
- `DirectusAuthService` for authentication workflows
- Interactive UI with server health checks
- Real-time data browsing from Directus collections
- Proper error handling and loading states

## Testing

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please ensure all tests pass before submitting PRs.

## License

This project is licensed under the MIT License.

## Resources

- [Directus Documentation](https://docs.directus.io)
- [Directus API Reference](https://docs.directus.io/reference/introduction.html)
- [Official JavaScript SDK](https://docs.directus.io/guides/sdk/getting-started.html)

## Support

For issues and questions:
- [GitHub Issues](https://github.com/yourusername/Directus.Net/issues)
- [Directus Community](https://directus.io/community)
