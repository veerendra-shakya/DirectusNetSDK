using Directus.Net.Storage;
using FluentAssertions;

namespace Directus.Net.Tests;

public class TokenStoreTests
{
    [Fact]
    public async Task InMemoryTokenStore_Should_Store_And_Retrieve_AccessToken()
    {
        var store = new InMemoryTokenStore();
        var token = "test-access-token";

        await store.SetAccessTokenAsync(token);
        var retrieved = await store.GetAccessTokenAsync();

        retrieved.Should().Be(token);
    }

    [Fact]
    public async Task InMemoryTokenStore_Should_Store_And_Retrieve_RefreshToken()
    {
        var store = new InMemoryTokenStore();
        var token = "test-refresh-token";

        await store.SetRefreshTokenAsync(token);
        var retrieved = await store.GetRefreshTokenAsync();

        retrieved.Should().Be(token);
    }

    [Fact]
    public async Task InMemoryTokenStore_Should_Clear_Tokens()
    {
        var store = new InMemoryTokenStore();
        
        await store.SetAccessTokenAsync("access-token");
        await store.SetRefreshTokenAsync("refresh-token");
        
        await store.ClearAsync();
        
        var accessToken = await store.GetAccessTokenAsync();
        var refreshToken = await store.GetRefreshTokenAsync();

        accessToken.Should().BeNull();
        refreshToken.Should().BeNull();
    }

    [Fact]
    public async Task InMemoryTokenStore_Should_Return_Null_For_Unset_Tokens()
    {
        var store = new InMemoryTokenStore();

        var accessToken = await store.GetAccessTokenAsync();
        var refreshToken = await store.GetRefreshTokenAsync();

        accessToken.Should().BeNull();
        refreshToken.Should().BeNull();
    }
}
