using Directus.Net.Exceptions;
using FluentAssertions;

namespace Directus.Net.Tests;

public class ExceptionTests
{
    [Fact]
    public void DirectusException_Should_Have_Message()
    {
        var exception = new DirectusException("Test error");

        exception.Message.Should().Be("Test error");
    }

    [Fact]
    public void DirectusException_Should_Have_StatusCode_And_ErrorCode()
    {
        var exception = new DirectusException("Test error", 404, "NOT_FOUND");

        exception.Message.Should().Be("Test error");
        exception.StatusCode.Should().Be(404);
        exception.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public void DirectusApiException_Should_Have_ErrorResponse()
    {
        var errorResponse = new { error = "details" };
        var exception = new DirectusApiException("API error", 500, "SERVER_ERROR", errorResponse);

        exception.Message.Should().Be("API error");
        exception.StatusCode.Should().Be(500);
        exception.ErrorCode.Should().Be("SERVER_ERROR");
        exception.ErrorResponse.Should().Be(errorResponse);
    }

    [Fact]
    public void DirectusAuthException_Should_Have_401_StatusCode()
    {
        var exception = new DirectusAuthException("Unauthorized");

        exception.Message.Should().Be("Unauthorized");
        exception.StatusCode.Should().Be(401);
        exception.ErrorCode.Should().Be("UNAUTHORIZED");
    }
}
