using Directus.Net;
using Directus.Net.Models;
using Microsoft.Extensions.Logging;

Console.WriteLine("=== Directus.Net SDK Sample Application ===\n");

var baseUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL") ?? "https://demo.directus.io";
Console.WriteLine($"Connecting to: {baseUrl}");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var client = new DirectusClient(baseUrl, new DirectusClientOptions
{
    Timeout = TimeSpan.FromSeconds(30)
}, loggerFactory);

try
{
    Console.WriteLine("\n--- Server Health Check ---");
    var isHealthy = await client.Utils.HealthCheckAsync();
    Console.WriteLine($"Server is {(isHealthy ? "healthy" : "not healthy")}");

    Console.WriteLine("\n--- Server Information ---");
    var serverInfo = await client.Utils.GetServerInfoAsync();
    Console.WriteLine($"Project Name: {serverInfo.Project?.ProjectName ?? "N/A"}");

    var email = Environment.GetEnvironmentVariable("DIRECTUS_EMAIL");
    var password = Environment.GetEnvironmentVariable("DIRECTUS_PASSWORD");

    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
    {
        Console.WriteLine("\n--- Authentication Example ---");
        try
        {
            var authResponse = await client.Auth.LoginAsync(email, password);
            Console.WriteLine($"Login successful! Access token: {authResponse.AccessToken?[..10]}...");

            Console.WriteLine("\n--- Get Current User ---");
            var currentUser = await client.Users.GetMeAsync();
            Console.WriteLine($"Current user: {currentUser?.Email}");

            Console.WriteLine("\n--- Logout ---");
            await client.Auth.LogoutAsync();
            Console.WriteLine("Logged out successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication error: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("\n--- Authentication Skipped ---");
        Console.WriteLine("Set DIRECTUS_EMAIL and DIRECTUS_PASSWORD environment variables to test authentication");
    }

    Console.WriteLine("\n--- Query Builder Example ---");
    var query = new QueryBuilder()
        .Limit(5)
        .Sort("-date_created")
        .Fields("id", "title", "status")
        .WithMeta("total_count")
        .Build();
    
    Console.WriteLine($"Query configured: limit={query.Limit}, sort={string.Join(",", query.Sort ?? Array.Empty<string>())}");

    Console.WriteLine("\n--- Filter Examples ---");
    var statusFilter = new { status = Filter.Eq("published") };
    Console.WriteLine($"Status filter created: status = 'published'");

    var combinedFilter = Filter.And(
        new { status = Filter.Eq("published") },
        new { views = Filter.Gt(100) }
    );
    Console.WriteLine("Combined filter created: status = 'published' AND views > 100");

    Console.WriteLine("\n=== Sample Application Completed Successfully ===");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    return 1;
}

return 0;
