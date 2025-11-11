
# Blazor Server Integration Guide with Directus.Net

A complete guide to integrating Directus.Net SDK with Blazor Server applications using .NET 9, including authentication, authorization, and secure token management.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Project Setup](#project-setup)
3. [SDK Installation](#sdk-installation)
4. [Service Configuration](#service-configuration)
5. [Authentication Implementation](#authentication-implementation)
6. [Authorization & Role-Based Access](#authorization--role-based-access)
7. [Secure Token Storage](#secure-token-storage)
8. [Two-Factor Authentication (2FA)](#two-factor-authentication-2fa)
9. [User Profile Management](#user-profile-management)
10. [Best Practices](#best-practices)

## Prerequisites

- .NET 9.0 SDK installed
- A Directus instance (cloud or self-hosted)
- Directus admin credentials
- Basic understanding of Blazor Server and ASP.NET Core

## Project Setup

### Creating a New Blazor Server Application

```bash
dotnet new blazor -o DirectusBlazorApp -f net9.0 -int Server
cd DirectusBlazorApp
```

### Project Structure

Your application will have this structure:

```
DirectusBlazorApp/
├── Components/
│   ├── Account/
│   ├── Layout/
│   └── Pages/
├── Data/
├── wwwroot/
├── Program.cs
└── appsettings.json
```

## SDK Installation

Install the Directus.Net SDK:

```bash
dotnet add package Directus.Net
```

Install additional required packages:

```bash
dotnet add package Microsoft.AspNetCore.Components.Server
dotnet add package Microsoft.AspNetCore.Authentication.Cookies
dotnet add package Blazored.LocalStorage
dotnet add package Blazored.SessionStorage
```

## Service Configuration

### 1. Configure appsettings.json

Add Directus configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Directus": {
    "BaseUrl": "https://your-instance.directus.app",
    "Timeout": 30
  },
  "Authentication": {
    "CookieExpiration": 30,
    "RequireHttps": true
  }
}
```

### 2. Create Configuration Models

Create `Models/DirectusSettings.cs`:

```csharp
namespace DirectusBlazorApp.Models;

public class DirectusSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;
}

public class AuthenticationSettings
{
    public int CookieExpiration { get; set; } = 30;
    public bool RequireHttps { get; set; } = true;
}
```

### 3. Create Custom Token Store for Blazor

Create `Services/BlazorServerTokenStore.cs`:

```csharp
using Directus.Net.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace DirectusBlazorApp.Services;

public class BlazorServerTokenStore : ITokenStore
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly ILogger<BlazorServerTokenStore> _logger;
    
    private const string AccessTokenKey = "directus_access_token";
    private const string RefreshTokenKey = "directus_refresh_token";

    public BlazorServerTokenStore(
        ProtectedSessionStorage sessionStorage,
        ILogger<BlazorServerTokenStore> logger)
    {
        _sessionStorage = sessionStorage;
        _logger = logger;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(AccessTokenKey);
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving access token");
            return null;
        }
    }

    public async Task SetAccessTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                await _sessionStorage.DeleteAsync(AccessTokenKey);
            }
            else
            {
                await _sessionStorage.SetAsync(AccessTokenKey, token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting access token");
        }
    }

    public async Task<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<string>(RefreshTokenKey);
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving refresh token");
            return null;
        }
    }

    public async Task SetRefreshTokenAsync(string? token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                await _sessionStorage.DeleteAsync(RefreshTokenKey);
            }
            else
            {
                await _sessionStorage.SetAsync(RefreshTokenKey, token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting refresh token");
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _sessionStorage.DeleteAsync(AccessTokenKey);
            await _sessionStorage.DeleteAsync(RefreshTokenKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing tokens");
        }
    }
}
```

### 4. Configure Services in Program.cs

Update `Program.cs`:

```csharp
using DirectusBlazorApp.Components;
using DirectusBlazorApp.Models;
using DirectusBlazorApp.Services;
using Directus.Net.Abstractions;
using Directus.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Directus settings
var directusSettings = builder.Configuration
    .GetSection("Directus")
    .Get<DirectusSettings>() ?? new DirectusSettings();

var authSettings = builder.Configuration
    .GetSection("Authentication")
    .Get<AuthenticationSettings>() ?? new AuthenticationSettings();

builder.Services.AddSingleton(directusSettings);
builder.Services.AddSingleton(authSettings);

// Register protected browser storage
builder.Services.AddScoped<ProtectedSessionStorage>();

// Register custom token store
builder.Services.AddScoped<ITokenStore, BlazorServerTokenStore>();

// Register Directus client
builder.Services.AddScoped<IDirectusClient>(sp =>
{
    var tokenStore = sp.GetRequiredService<ITokenStore>();
    var logger = sp.GetRequiredService<ILogger<DirectusClient>>();
    
    return new DirectusClient(
        directusSettings.BaseUrl,
        tokenStore: tokenStore,
        logger: logger
    );
});

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(authSettings.CookieExpiration);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = authSettings.RequireHttps 
            ? CookieSecurePolicy.Always 
            : CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run("http://0.0.0.0:5000");
```

## Authentication Implementation

### 1. Create Authentication Service

Create `Services/AuthenticationService.cs`:

```csharp
using System.Security.Claims;
using Directus.Net.Abstractions;
using Directus.Net.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DirectusBlazorApp.Services;

public class AuthenticationService
{
    private readonly IDirectusClient _directusClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IDirectusClient directusClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationService> logger)
    {
        _directusClient = directusClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {
            var response = await _directusClient.Auth.LoginAsync(email, password);
            
            if (response?.AccessToken == null)
            {
                return false;
            }

            // Get user info
            var userInfo = await _directusClient.Users.GetCurrentUserAsync();
            
            if (userInfo == null)
            {
                return false;
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id ?? string.Empty),
                new Claim(ClaimTypes.Email, userInfo.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{userInfo.FirstName} {userInfo.LastName}".Trim()),
            };

            // Add role claim if exists
            if (!string.IsNullOrEmpty(userInfo.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, userInfo.Role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe 
                    ? DateTimeOffset.UtcNow.AddDays(30) 
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            return true;
        }
        catch (DirectusException ex)
        {
            _logger.LogError(ex, "Login failed for user {Email}", email);
            return false;
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
            _logger.LogError(ex, "Error during Directus logout");
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<bool> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        try
        {
            var newUser = new
            {
                email,
                password,
                first_name = firstName,
                last_name = lastName,
                role = "user" // Default role
            };

            await _directusClient.Users.CreateUserAsync(newUser);
            return true;
        }
        catch (DirectusException ex)
        {
            _logger.LogError(ex, "Registration failed for email {Email}", email);
            return false;
        }
    }
}
```

Register the service in `Program.cs`:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationService>();
```

### 2. Create Login Page

Create `Components/Pages/Login.razor`:

```razor
@page "/login"
@using DirectusBlazorApp.Services
@inject AuthenticationService AuthService
@inject NavigationManager Navigation

<PageTitle>Login</PageTitle>

<div class="login-container">
    <div class="login-card">
        <h3>Login to Directus</h3>
        
        @if (!string.IsNullOrEmpty(errorMessage))
        {
            <div class="alert alert-danger">@errorMessage</div>
        }

        <EditForm Model="loginModel" OnValidSubmit="HandleLogin">
            <DataAnnotationsValidator />
            <ValidationSummary />

            <div class="form-group">
                <label for="email">Email</label>
                <InputText id="email" @bind-Value="loginModel.Email" class="form-control" />
            </div>

            <div class="form-group">
                <label for="password">Password</label>
                <InputText id="password" type="password" @bind-Value="loginModel.Password" class="form-control" />
            </div>

            <div class="form-check">
                <InputCheckbox id="rememberMe" @bind-Value="loginModel.RememberMe" class="form-check-input" />
                <label class="form-check-label" for="rememberMe">Remember me</label>
            </div>

            <button type="submit" class="btn btn-primary" disabled="@isLoading">
                @if (isLoading)
                {
                    <span>Logging in...</span>
                }
                else
                {
                    <span>Login</span>
                }
            </button>
        </EditForm>

        <div class="mt-3">
            <p>Don't have an account? <a href="/register">Register here</a></p>
        </div>
    </div>
</div>

@code {
    private LoginModel loginModel = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var success = await AuthService.LoginAsync(
                loginModel.Email, 
                loginModel.Password, 
                loginModel.RememberMe);

            if (success)
            {
                Navigation.NavigateTo("/", forceLoad: true);
            }
            else
            {
                errorMessage = "Invalid email or password.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = "An error occurred during login. Please try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
```

### 3. Create Register Page

Create `Components/Pages/Register.razor`:

```razor
@page "/register"
@using DirectusBlazorApp.Services
@inject AuthenticationService AuthService
@inject NavigationManager Navigation

<PageTitle>Register</PageTitle>

<div class="register-container">
    <div class="register-card">
        <h3>Create Account</h3>
        
        @if (!string.IsNullOrEmpty(errorMessage))
        {
            <div class="alert alert-danger">@errorMessage</div>
        }

        @if (!string.IsNullOrEmpty(successMessage))
        {
            <div class="alert alert-success">@successMessage</div>
        }

        <EditForm Model="registerModel" OnValidSubmit="HandleRegister">
            <DataAnnotationsValidator />
            <ValidationSummary />

            <div class="form-group">
                <label for="firstName">First Name</label>
                <InputText id="firstName" @bind-Value="registerModel.FirstName" class="form-control" />
            </div>

            <div class="form-group">
                <label for="lastName">Last Name</label>
                <InputText id="lastName" @bind-Value="registerModel.LastName" class="form-control" />
            </div>

            <div class="form-group">
                <label for="email">Email</label>
                <InputText id="email" @bind-Value="registerModel.Email" class="form-control" />
            </div>

            <div class="form-group">
                <label for="password">Password</label>
                <InputText id="password" type="password" @bind-Value="registerModel.Password" class="form-control" />
            </div>

            <div class="form-group">
                <label for="confirmPassword">Confirm Password</label>
                <InputText id="confirmPassword" type="password" @bind-Value="registerModel.ConfirmPassword" class="form-control" />
            </div>

            <button type="submit" class="btn btn-primary" disabled="@isLoading">
                @if (isLoading)
                {
                    <span>Registering...</span>
                }
                else
                {
                    <span>Register</span>
                }
            </button>
        </EditForm>

        <div class="mt-3">
            <p>Already have an account? <a href="/login">Login here</a></p>
        </div>
    </div>
</div>

@code {
    private RegisterModel registerModel = new();
    private string? errorMessage;
    private string? successMessage;
    private bool isLoading;

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = null;
        successMessage = null;

        try
        {
            if (registerModel.Password != registerModel.ConfirmPassword)
            {
                errorMessage = "Passwords do not match.";
                return;
            }

            var success = await AuthService.RegisterAsync(
                registerModel.Email,
                registerModel.Password,
                registerModel.FirstName,
                registerModel.LastName);

            if (success)
            {
                successMessage = "Registration successful! Redirecting to login...";
                await Task.Delay(2000);
                Navigation.NavigateTo("/login");
            }
            else
            {
                errorMessage = "Registration failed. Email may already be in use.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = "An error occurred during registration. Please try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    public class RegisterModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
```

### 4. Create Logout Component

Create `Components/Pages/Logout.razor`:

```razor
@page "/logout"
@using DirectusBlazorApp.Services
@inject AuthenticationService AuthService
@inject NavigationManager Navigation

@code {
    protected override async Task OnInitializedAsync()
    {
        await AuthService.LogoutAsync();
        Navigation.NavigateTo("/login", forceLoad: true);
    }
}
```

## Authorization & Role-Based Access

### 1. Protect Routes with AuthorizeView

Create `Components/Pages/Dashboard.razor`:

```razor
@page "/dashboard"
@attribute [Authorize]
@using Microsoft.AspNetCore.Authorization
@using Directus.Net.Abstractions

<PageTitle>Dashboard</PageTitle>

<AuthorizeView>
    <Authorized>
        <h3>Welcome, @context.User.Identity?.Name!</h3>
        
        <div class="user-info">
            <p><strong>Email:</strong> @context.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value</p>
            <p><strong>Role:</strong> @context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value</p>
        </div>

        <AuthorizeView Roles="admin">
            <Authorized>
                <div class="admin-section">
                    <h4>Admin Controls</h4>
                    <p>This section is only visible to administrators.</p>
                </div>
            </Authorized>
        </AuthorizeView>
    </Authorized>
    <NotAuthorized>
        <p>You must be logged in to view this page.</p>
        <a href="/login">Login</a>
    </NotAuthorized>
</AuthorizeView>
```

### 2. Role-Based Authorization Policy

Add policies to `Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("admin"));
    
    options.AddPolicy("UserOrAdmin", policy => 
        policy.RequireRole("user", "admin"));
    
    options.AddPolicy("RequireEmailVerified", policy =>
        policy.RequireClaim("email_verified", "true"));
});
```

Use in components:

```razor
@attribute [Authorize(Policy = "AdminOnly")]
```

## Secure Token Storage

The `BlazorServerTokenStore` we created earlier uses ASP.NET Core's `ProtectedSessionStorage`, which:

- Encrypts data using Data Protection APIs
- Stores tokens in server-side session storage
- Automatically handles token lifecycle
- Protects against XSS attacks

### Additional Security Measures

Create `Services/SecureStorageService.cs` for sensitive data:

```csharp
using Microsoft.AspNetCore.DataProtection;

namespace DirectusBlazorApp.Services;

public class SecureStorageService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly ILogger<SecureStorageService> _logger;

    public SecureStorageService(
        IDataProtectionProvider dataProtectionProvider,
        ILogger<SecureStorageService> logger)
    {
        _dataProtectionProvider = dataProtectionProvider;
        _logger = logger;
    }

    public string Protect(string data, string purpose)
    {
        var protector = _dataProtectionProvider.CreateProtector(purpose);
        return protector.Protect(data);
    }

    public string Unprotect(string protectedData, string purpose)
    {
        var protector = _dataProtectionProvider.CreateProtector(purpose);
        return protector.Unprotect(protectedData);
    }
}
```

Register in `Program.cs`:

```csharp
builder.Services.AddDataProtection();
builder.Services.AddScoped<SecureStorageService>();
```

## Two-Factor Authentication (2FA)

### 1. Enable 2FA Setup

Create `Services/TwoFactorService.cs`:

```csharp
using Directus.Net.Abstractions;

namespace DirectusBlazorApp.Services;

public class TwoFactorService
{
    private readonly IDirectusClient _directusClient;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(
        IDirectusClient directusClient,
        ILogger<TwoFactorService> logger)
    {
        _directusClient = directusClient;
        _logger = logger;
    }

    public async Task<TwoFactorSetupResponse?> GenerateTwoFactorSecretAsync()
    {
        try
        {
            var response = await _directusClient.Auth.GenerateTwoFactorSecretAsync();
            return new TwoFactorSetupResponse
            {
                Secret = response.Secret,
                QrCode = response.QrCodeUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating 2FA secret");
            return null;
        }
    }

    public async Task<bool> EnableTwoFactorAsync(string secret, string otp)
    {
        try
        {
            await _directusClient.Auth.EnableTwoFactorAsync(secret, otp);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling 2FA");
            return false;
        }
    }

    public async Task<bool> DisableTwoFactorAsync(string otp)
    {
        try
        {
            await _directusClient.Auth.DisableTwoFactorAsync(otp);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling 2FA");
            return false;
        }
    }

    public async Task<bool> VerifyTwoFactorAsync(string otp)
    {
        try
        {
            await _directusClient.Auth.VerifyTwoFactorAsync(otp);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying 2FA code");
            return false;
        }
    }
}

public class TwoFactorSetupResponse
{
    public string Secret { get; set; } = string.Empty;
    public string QrCode { get; set; } = string.Empty;
}
```

### 2. Create 2FA Setup Page

Create `Components/Pages/TwoFactorSetup.razor`:

```razor
@page "/account/two-factor-setup"
@attribute [Authorize]
@using DirectusBlazorApp.Services
@inject TwoFactorService TwoFactorService

<PageTitle>Two-Factor Authentication Setup</PageTitle>

<div class="two-factor-setup">
    <h3>Enable Two-Factor Authentication</h3>

    @if (setupData == null && !isLoading)
    {
        <button class="btn btn-primary" @onclick="GenerateSecret">
            Generate 2FA Secret
        </button>
    }

    @if (isLoading)
    {
        <p>Loading...</p>
    }

    @if (setupData != null)
    {
        <div class="setup-instructions">
            <h4>Step 1: Scan QR Code</h4>
            <p>Use an authenticator app (Google Authenticator, Authy, etc.) to scan this QR code:</p>
            <img src="@setupData.QrCode" alt="2FA QR Code" />

            <h4>Step 2: Enter Verification Code</h4>
            <div class="form-group">
                <label for="otp">6-digit code from your authenticator app:</label>
                <input id="otp" type="text" @bind="otpCode" class="form-control" maxlength="6" />
            </div>

            <button class="btn btn-success" @onclick="EnableTwoFactor" disabled="@string.IsNullOrEmpty(otpCode)">
                Verify and Enable 2FA
            </button>
        </div>
    }

    @if (!string.IsNullOrEmpty(message))
    {
        <div class="alert alert-info mt-3">@message</div>
    }
</div>

@code {
    private TwoFactorSetupResponse? setupData;
    private string otpCode = string.Empty;
    private string? message;
    private bool isLoading;

    private async Task GenerateSecret()
    {
        isLoading = true;
        setupData = await TwoFactorService.GenerateTwoFactorSecretAsync();
        isLoading = false;

        if (setupData == null)
        {
            message = "Error generating 2FA secret. Please try again.";
        }
    }

    private async Task EnableTwoFactor()
    {
        if (setupData == null || string.IsNullOrEmpty(otpCode))
            return;

        var success = await TwoFactorService.EnableTwoFactorAsync(setupData.Secret, otpCode);
        
        if (success)
        {
            message = "Two-factor authentication has been enabled successfully!";
            setupData = null;
            otpCode = string.Empty;
        }
        else
        {
            message = "Invalid verification code. Please try again.";
        }
    }
}
```

## User Profile Management

### 1. Create User Profile Service

Create `Services/UserProfileService.cs`:

```csharp
using Directus.Net.Abstractions;

namespace DirectusBlazorApp.Services;

public class UserProfileService
{
    private readonly IDirectusClient _directusClient;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        IDirectusClient directusClient,
        ILogger<UserProfileService> logger)
    {
        _directusClient = directusClient;
        _logger = logger;
    }

    public async Task<UserProfile?> GetCurrentUserProfileAsync()
    {
        try
        {
            var user = await _directusClient.Users.GetCurrentUserAsync();
            
            if (user == null)
                return null;

            return new UserProfile
            {
                Id = user.Id ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Role = user.Role ?? string.Empty,
                Avatar = user.Avatar,
                Location = user.Location,
                Title = user.Title,
                Description = user.Description
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile");
            return null;
        }
    }

    public async Task<bool> UpdateProfileAsync(UserProfile profile)
    {
        try
        {
            var updateData = new
            {
                first_name = profile.FirstName,
                last_name = profile.LastName,
                location = profile.Location,
                title = profile.Title,
                description = profile.Description
            };

            await _directusClient.Users.UpdateUserAsync(profile.Id, updateData);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            await _directusClient.Users.UpdateCurrentUserPasswordAsync(currentPassword, newPassword);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return false;
        }
    }
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Location { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}
```

### 2. Create Profile Page

Create `Components/Pages/Profile.razor`:

```razor
@page "/account/profile"
@attribute [Authorize]
@using DirectusBlazorApp.Services
@inject UserProfileService ProfileService

<PageTitle>My Profile</PageTitle>

<div class="profile-container">
    <h3>My Profile</h3>

    @if (profile == null && !isLoading)
    {
        <p>Unable to load profile.</p>
    }

    @if (isLoading)
    {
        <p>Loading profile...</p>
    }

    @if (profile != null)
    {
        <EditForm Model="profile" OnValidSubmit="SaveProfile">
            <div class="form-group">
                <label>Email</label>
                <input type="text" value="@profile.Email" class="form-control" disabled />
                <small class="form-text text-muted">Email cannot be changed</small>
            </div>

            <div class="form-group">
                <label for="firstName">First Name</label>
                <InputText id="firstName" @bind-Value="profile.FirstName" class="form-control" />
            </div>

            <div class="form-group">
                <label for="lastName">Last Name</label>
                <InputText id="lastName" @bind-Value="profile.LastName" class="form-control" />
            </div>

            <div class="form-group">
                <label for="title">Title</label>
                <InputText id="title" @bind-Value="profile.Title" class="form-control" />
            </div>

            <div class="form-group">
                <label for="location">Location</label>
                <InputText id="location" @bind-Value="profile.Location" class="form-control" />
            </div>

            <div class="form-group">
                <label for="description">Bio</label>
                <InputTextArea id="description" @bind-Value="profile.Description" class="form-control" rows="4" />
            </div>

            <button type="submit" class="btn btn-primary" disabled="@isSaving">
                @(isSaving ? "Saving..." : "Save Changes")
            </button>
        </EditForm>

        @if (!string.IsNullOrEmpty(message))
        {
            <div class="alert alert-success mt-3">@message</div>
        }

        <hr />

        <h4>Change Password</h4>
        <EditForm Model="passwordModel" OnValidSubmit="ChangePassword">
            <div class="form-group">
                <label for="currentPassword">Current Password</label>
                <InputText id="currentPassword" type="password" @bind-Value="passwordModel.CurrentPassword" class="form-control" />
            </div>

            <div class="form-group">
                <label for="newPassword">New Password</label>
                <InputText id="newPassword" type="password" @bind-Value="passwordModel.NewPassword" class="form-control" />
            </div>

            <div class="form-group">
                <label for="confirmPassword">Confirm New Password</label>
                <InputText id="confirmPassword" type="password" @bind-Value="passwordModel.ConfirmPassword" class="form-control" />
            </div>

            <button type="submit" class="btn btn-warning">Change Password</button>
        </EditForm>

        @if (!string.IsNullOrEmpty(passwordMessage))
        {
            <div class="alert alert-info mt-3">@passwordMessage</div>
        }
    }
</div>

@code {
    private UserProfile? profile;
    private PasswordChangeModel passwordModel = new();
    private string? message;
    private string? passwordMessage;
    private bool isLoading = true;
    private bool isSaving;

    protected override async Task OnInitializedAsync()
    {
        profile = await ProfileService.GetCurrentUserProfileAsync();
        isLoading = false;
    }

    private async Task SaveProfile()
    {
        if (profile == null)
            return;

        isSaving = true;
        var success = await ProfileService.UpdateProfileAsync(profile);
        
        if (success)
        {
            message = "Profile updated successfully!";
        }
        else
        {
            message = "Error updating profile. Please try again.";
        }

        isSaving = false;
    }

    private async Task ChangePassword()
    {
        if (passwordModel.NewPassword != passwordModel.ConfirmPassword)
        {
            passwordMessage = "New passwords do not match.";
            return;
        }

        var success = await ProfileService.ChangePasswordAsync(
            passwordModel.CurrentPassword,
            passwordModel.NewPassword);

        if (success)
        {
            passwordMessage = "Password changed successfully!";
            passwordModel = new();
        }
        else
        {
            passwordMessage = "Error changing password. Please check your current password.";
        }
    }

    public class PasswordChangeModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
```

## Best Practices

### 1. Security Best Practices

**Always use HTTPS in production:**

```csharp
// In Program.cs
if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

**Implement Content Security Policy:**

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
    await next();
});
```

**Enable Anti-Forgery Tokens:**

Already included with `app.UseAntiforgery()` in the middleware pipeline.

### 2. Performance Optimization

**Implement response caching:**

```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

**Use circuit breaker pattern for Directus calls:**

```csharp
builder.Services.AddHttpClient("DirectusClient")
    .AddTransientHttpErrorPolicy(policy => 
        policy.WaitAndRetryAsync(3, retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddTransientHttpErrorPolicy(policy => 
        policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
```

### 3. Error Handling

Create a global error boundary in `Components/Layout/ErrorBoundary.razor`:

```razor
@inherits ErrorBoundary

<div class="error-boundary">
    @if (CurrentException != null)
    {
        <div class="alert alert-danger">
            <h4>An error occurred</h4>
            <p>@CurrentException.Message</p>
            
            @if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                <pre>@CurrentException.StackTrace</pre>
            }
            
            <button @onclick="Recover" class="btn btn-primary">Try Again</button>
        </div>
    }
    else
    {
        @ChildContent
    }
</div>

@code {
    private void Recover()
    {
        Recover();
    }
}
```

### 4. Logging Configuration

Configure comprehensive logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "DirectusBlazorApp": "Debug",
      "Directus.Net": "Information"
    }
  }
}
```

### 5. State Management

For complex state, consider implementing a state container:

```csharp
public class AppState
{
    private UserProfile? _currentUser;
    
    public UserProfile? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
```

Register as scoped service:

```csharp
builder.Services.AddScoped<AppState>();
```

## Running the Application

### Development

```bash
dotnet run
```

Access at: `http://0.0.0.0:5000`

### Production Deployment on Replit

1. Configure deployment settings in the Replit UI
2. Set environment variables for production
3. Deploy using the Reserved VM Deployment option

### Testing Checklist

- [ ] User registration works
- [ ] Login/logout functionality
- [ ] Protected routes redirect unauthenticated users
- [ ] Role-based access control works
- [ ] Profile updates persist
- [ ] Password changes work
- [ ] 2FA setup and verification
- [ ] Token refresh on expiration
- [ ] Secure storage protects sensitive data

## Troubleshooting

### Common Issues

**Issue: "Protected storage is unavailable"**
- Solution: Ensure you're using `ProtectedSessionStorage` with server-side rendering

**Issue: "Unauthorized" errors after login**
- Solution: Check that cookies are being set correctly and HTTPS is configured properly

**Issue: Token refresh fails**
- Solution: Verify refresh token is being stored and retrieved correctly

**Issue: 2FA setup fails**
- Solution: Ensure your Directus instance has 2FA enabled in settings

## Conclusion

You now have a complete, production-ready Blazor Server application with:

- Secure authentication and authorization
- Role-based access control
- Two-factor authentication support
- User profile management
- Protected token storage
- Best practices for security and performance

For additional features, refer to the main [Directus.Net documentation](README.md) and [Getting Started Guide](GETTING_STARTED.md).
