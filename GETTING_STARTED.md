
# Getting Started with Directus.Net

This guide will help you get started with the Directus.Net SDK, whether you're using Visual Studio, Visual Studio Code, or Replit.

## Prerequisites

- .NET 9.0 SDK or later
- A Directus instance (cloud or self-hosted)
- Your Directus instance URL and credentials

## Option 1: Getting Started in Visual Studio

### 1. Clone or Open the Project

```bash
git clone <your-repository-url>
cd Directus.Net
```

Or open the `Directus.Net.sln` solution file directly in Visual Studio.

### 2. Restore NuGet Packages

Visual Studio should automatically restore packages. If not:
- Right-click the solution in Solution Explorer
- Select "Restore NuGet Packages"

Or use the Package Manager Console:
```powershell
dotnet restore
```

### 3. Build the Solution

- Press `Ctrl+Shift+B` or
- Go to Build → Build Solution

### 4. Run the Tests

- Open Test Explorer (Test → Test Explorer)
- Click "Run All Tests"

Or use the command line:
```bash
dotnet test
```

### 5. Run the Sample Application

- Set `Directus.Net.Sample` as the startup project (right-click → Set as Startup Project)
- Press `F5` to run with debugging or `Ctrl+F5` to run without debugging

Or from the command line:
```bash
dotnet run --project samples/Directus.Net.Sample
```

## Option 2: Getting Started in Replit

### 1. Fork or Import the Repl

- Click the "Run" button to build and test the project
- The workflow will automatically build the solution and run all tests

### 2. Run Tests

Click the "Run" button, which executes:
```bash
dotnet build Directus.Net.sln && dotnet test Directus.Net.sln --no-build
```

### 3. Run the Sample Application

Use the shell to run:
```bash
dotnet run --project samples/Directus.Net.Sample
```

## Using the SDK in Your Project

### Installation

Add the NuGet package to your project:

```bash
dotnet add package Directus.Net
```

Or add it to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Directus.Net" Version="1.0.0" />
</ItemGroup>
```

### Basic Usage Example

```csharp
using Directus.Net;
using Directus.Net.Models;

// Create a client instance
var client = new DirectusClient("https://your-directus-instance.com");

try
{
    // Login
    await client.Auth.LoginAsync("admin@example.com", "your-password");
    
    // Query items from a collection
    var query = new QueryBuilder()
        .Limit(10)
        .Sort("-date_created")
        .Fields("id", "title", "status")
        .Build();
    
    var articles = await client.Items.ReadItemsAsync<Article>("articles", query);
    
    foreach (var article in articles)
    {
        Console.WriteLine($"{article.Id}: {article.Title}");
    }
    
    // Logout
    await client.Auth.LogoutAsync();
}
catch (DirectusException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### With Dependency Injection

```csharp
using Directus.Net.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register Directus client
builder.Services.AddDirectus("https://your-directus-instance.com", options =>
{
    options.Timeout = TimeSpan.FromSeconds(30);
});

var host = builder.Build();

// Use the client
var client = host.Services.GetRequiredService<IDirectusClient>();
await client.Auth.LoginAsync("admin@example.com", "password");
```

## Quick Testing Checklist

### 1. Unit Tests
All tests should pass when you run:
```bash
dotnet test
```

Current test coverage includes:
- ✅ Query Builder tests (13 tests)
- ✅ Token Store tests (4 tests)
- ✅ Exception tests (3 tests)

### 2. Sample Application
The sample demonstrates:
- Health check verification
- Authentication
- Server info retrieval
- Query building with filters
- Proper error handling

### 3. Manual Testing

Create a simple test file to verify your Directus connection:

```csharp
// TestConnection.cs
using Directus.Net;

var client = new DirectusClient("https://your-instance.directus.app");

try
{
    var health = await client.Utils.GetServerHealthAsync();
    Console.WriteLine($"Server Status: {health.Status}");
    
    await client.Auth.LoginAsync("your-email@example.com", "your-password");
    Console.WriteLine("Authentication successful!");
    
    var info = await client.Utils.GetServerInfoAsync();
    Console.WriteLine($"Directus Version: {info.Version}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

Run it with:
```bash
dotnet run TestConnection.cs
```

## Common Operations

### Creating Items

```csharp
var newArticle = await client.Items.CreateItemAsync("articles", new
{
    title = "My First Article",
    content = "Hello, Directus!",
    status = "draft"
});
```

### Reading Items with Filters

```csharp
var query = new QueryBuilder()
    .WithFilter(Filter.And(
        new { status = Filter.Eq("published") },
        new { views = Filter.Gt(100) }
    ))
    .Build();

var items = await client.Items.ReadItemsAsync<Article>("articles", query);
```

### Updating Items

```csharp
await client.Items.UpdateItemAsync("articles", "item-id", new
{
    status = "published",
    published_date = DateTime.UtcNow
});
```

### Deleting Items

```csharp
await client.Items.DeleteItemAsync("articles", "item-id");
```

### File Upload

```csharp
using var fileStream = File.OpenRead("photo.jpg");
var file = await client.Files.UploadFileAsync(
    "photo.jpg",
    fileStream,
    title: "My Photo"
);
```

## Troubleshooting

### Build Errors

If you get build errors:
1. Ensure you have .NET 9.0 SDK installed: `dotnet --version`
2. Clean and rebuild: `dotnet clean && dotnet build`
3. Restore packages: `dotnet restore`

### Test Failures

If tests fail:
1. Check that all dependencies are restored
2. Review the test output for specific errors
3. Ensure your .NET version is compatible

### Connection Issues

If you can't connect to Directus:
1. Verify your Directus instance URL is correct
2. Check that your credentials are valid
3. Ensure the Directus instance is accessible from your network
4. Check firewall/security settings

## Next Steps

1. **Explore the Sample** - Review `samples/Directus.Net.Sample/Program.cs` for practical examples
2. **Read the Documentation** - Check the [README.md](README.md) for detailed API documentation
3. **Review Tests** - Look at `tests/Directus.Net.Tests/` for usage patterns
4. **Build Your Application** - Start integrating Directus.Net into your project

## Additional Resources

- [Directus Documentation](https://docs.directus.io)
- [Directus API Reference](https://docs.directus.io/reference/introduction.html)
- [Official JavaScript SDK](https://docs.directus.io/guides/sdk/getting-started.html)

## Support

- **Issues**: Report bugs or request features via GitHub Issues
- **Questions**: Ask in the Directus Community or Replit Community Hub
- **Documentation**: Refer to inline XML documentation in the source code

Happy coding! 🚀
