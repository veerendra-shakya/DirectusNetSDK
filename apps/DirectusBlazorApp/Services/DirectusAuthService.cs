using Directus.Net.Abstractions;
using Directus.Net.Exceptions;

namespace DirectusBlazorApp.Services;

public class DirectusAuthService
{
    private readonly IDirectusClient _directusClient;
    private readonly ILogger<DirectusAuthService> _logger;

    public DirectusAuthService(
        IDirectusClient directusClient,
        ILogger<DirectusAuthService> logger)
    {
        _directusClient = directusClient;
        _logger = logger;
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("Attempting login for user: {Email}", email);
            var response = await _directusClient.Auth.LoginAsync(email, password);
            
            if (response?.AccessToken == null)
            {
                _logger.LogWarning("Login failed - no access token received for user: {Email}", email);
                return (false, "Login failed - no access token received");
            }

            _logger.LogInformation("Login successful for user: {Email}", email);
            return (true, null);
        }
        catch (DirectusAuthException ex)
        {
            _logger.LogError(ex, "Authentication failed for user {Email}", email);
            return (false, "Invalid email or password");
        }
        catch (DirectusApiException ex)
        {
            _logger.LogError(ex, "API error during login for user {Email}", email);
            return (false, $"Login error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for user {Email}", email);
            return (false, "An unexpected error occurred during login");
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _directusClient.Auth.LogoutAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
        }
    }

    public async Task<DirectusUser?> GetCurrentUserAsync()
    {
        try
        {
            return await _directusClient.Users.GetMeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current user");
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await _directusClient.Auth.GetTokenAsync();
            var isAuthenticated = !string.IsNullOrEmpty(token);
            _logger.LogInformation("Authentication check: {IsAuthenticated}, Token length: {TokenLength}", 
                isAuthenticated, token?.Length ?? 0);
            return isAuthenticated;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking authentication status");
            return false;
        }
    }
}
