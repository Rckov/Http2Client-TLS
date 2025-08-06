using FluentAssertions;

using Http2Client.Builders;
using Http2Client.Core.Enums;
using Http2Client.Core.Models;

namespace Http2Client.Test.Builders;

public class HttpRequestBuilderTests
{
    [Fact]
    public void NoUrl_Throws()
    {
        var builder = new HttpRequestBuilder();

        builder.Invoking(b => b.Build())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("RequestUrl is required*");
    }

    [Fact]
    public void Method_String_Works()
    {
        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithMethod("post")
            .Build();

        request.RequestMethod.Should().Be("POST");
    }

    [Fact]
    public void Method_Enum_Works()
    {
        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithMethod(HttpMethod.Delete)
            .Build();

        request.RequestMethod.Should().Be("DELETE");
    }

    [Fact]
    public void JsonBody_Works()
    {
        var data = new { name = "test", value = 123 };

        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithJsonBody(data)
            .Build();

        request.RequestBody.Should().Contain("test");
        request.Headers["Content-Type"].Should().Be("application/json");
    }

    [Fact]
    public void BinaryBody_Works()
    {
        var data = new byte[] { 1, 2, 3, 4 };

        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithBinaryBody(data)
            .Build();

        request.IsByteRequest.Should().BeTrue();
        request.RequestBody.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Timeout_Negative_Throws()
    {
        var builder = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithTimeout(TimeSpan.FromSeconds(-1));

        builder.Invoking(b => b.Build())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("TimeoutMilliseconds must be positive*");
    }

    [Fact]
    public void Chaining_Works()
    {
        var cookie = new ClientCookie { Name = "test", Value = "value" };

        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithMethod("POST")
            .WithHeader("X-Test", "value")
            .AddCookie(cookie)
            .WithBrowserType(BrowserType.Safari160)
            .WithProxy("socks5://proxy:1080", true)
            .Build();

        request.RequestUrl.Should().Be("https://example.com");
        request.RequestMethod.Should().Be("POST");
        request.Headers["X-Test"].Should().Be("value");
        request.RequestCookies.Should().ContainSingle().Which.Should().Be(cookie);
        request.BrowserType.Should().Be(BrowserType.Safari160);
        request.ProxyUrl.Should().Be("socks5://proxy:1080");
        request.IsRotatingProxy.Should().BeTrue();
    }

    [Fact]
    public void Custom_Works()
    {
        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .With(r => r.ForceHttp1 = true)
            .Build();

        request.ForceHttp1.Should().BeTrue();
    }
}