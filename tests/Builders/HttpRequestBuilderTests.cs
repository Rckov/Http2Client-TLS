using FluentAssertions;

using Http2Client.Builders;
using Http2Client.Core.Models;

using Xunit;

namespace Http2Client.Test.Builders;

public class HttpRequestBuilderTests
{
    [Fact]
    public void Build_NoUrl_Throws()
    {
        var builder = new HttpRequestBuilder();

        builder.Invoking(b => b.Build())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("RequestUrl is required*");
    }

    [Fact]
    public void WithUrl_Invalid_Throws()
    {
        var builder = new HttpRequestBuilder();
        builder.Invoking(b => b.WithUrl("invalid-url")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithMethod_Null_Throws()
    {
        var builder = new HttpRequestBuilder();
        builder.Invoking(b => b.WithMethod(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithJsonBody_Null_Throws()
    {
        var builder = new HttpRequestBuilder();
        builder.Invoking(b => b.WithJsonBody<object>(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithBinaryBody_Null_Throws()
    {
        var builder = new HttpRequestBuilder();
        builder.Invoking(b => b.WithBinaryBody(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithLocalAddress_Invalid_Throws()
    {
        var builder = new HttpRequestBuilder();

        builder.Invoking(b => b.WithLocalAddress("invalid-ip"))
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Local address must be a valid IP address.*");
    }

    [Fact]
    public void Method_String_Works()
    {
        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithMethod(HttpMethod.Post)
            .Build();

        request.RequestMethod.Should().Be("POST");
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
    public void WithTimeout_Negative_Throws()
    {
        var builder = new HttpRequestBuilder();

        builder.Invoking(b => b.WithTimeout(TimeSpan.FromSeconds(-1)))
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Timeout must be greater than zero.*");
    }

    [Fact]
    public void WithTimeout_TooLarge_Throws()
    {
        var builder = new HttpRequestBuilder();

        builder.Invoking(b => b.WithTimeout(TimeSpan.FromHours(1)))
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Timeout cannot exceed 30 minutes.*");
    }

    [Fact]
    public void Chaining_Works()
    {
        var cookie = new ClientCookie { Name = "test", Value = "value" };

        var request = new HttpRequestBuilder()
            .WithUrl("https://example.com")
            .WithMethod(HttpMethod.Post)
            .AddCookie(cookie)
            .Build();

        request.RequestUrl.Should().Be("https://example.com");
        request.RequestMethod.Should().Be("POST");
        request.RequestCookies.Should().ContainSingle().Which.Should().Be(cookie);
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