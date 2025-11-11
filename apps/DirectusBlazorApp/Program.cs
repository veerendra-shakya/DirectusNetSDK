using DirectusBlazorApp.Components;
using DirectusBlazorApp.Services;
using Directus.Net;
using Directus.Net.Abstractions;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var directusUrl = builder.Configuration.GetValue<string>("Directus:BaseUrl") 
    ?? "https://data.gwaliorsmartcity.org";

builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddScoped<ITokenStore, BlazorServerTokenStore>();

builder.Services.AddScoped<IDirectusClient>(sp =>
{
    var tokenStore = sp.GetRequiredService<ITokenStore>();
    var loggerFactory = sp.GetService<ILoggerFactory>();

    return new DirectusClient(directusUrl, new DirectusClientOptions
    {
        Timeout = TimeSpan.FromSeconds(30)
    }, loggerFactory);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DirectusAuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run("http://0.0.0.0:5000");
