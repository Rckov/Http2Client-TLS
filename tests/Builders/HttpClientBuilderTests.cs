using FluentAssertions;

using Http2Client.Builders;
using Http2Client.Core.Enums;

using Xunit;

namespace Http2Client.Test.Builders;

public class HttpClientBuilderTests
{
    [Fact]
    public void WithSessionId_Empty_Throws()
    {
        var builder = new HttpClientBuilder();

        builder.Invoking(b => b.WithSessionId(Guid.Empty))
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("SessionId must be non-empty.*");
    }

    [Fact]
    public void WithCustomHttp2Client_Null_Throws()
    {
        var builder = new HttpClientBuilder();
        builder.Invoking(b => b.WithCustomHttp2Client(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithHeader_NullName_Throws()
    {
        var builder = new HttpClientBuilder();
        builder.Invoking(b => b.WithHeader(null!, "value")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithHeaderOrder_Null_Throws()
    {
        var builder = new HttpClientBuilder();
        builder.Invoking(b => b.WithHeaderOrder(null!)).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithUserAgent_Null_Throws()
    {
        var builder = new HttpClientBuilder();
        builder.Invoking(b => b.WithUserAgent(null!)).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Chaining_Works()
    {
        var options = new HttpClientBuilder()
            .WithLibraryPath(TestConstants.LibraryPath)
            .WithBrowserType(BrowserType.Firefox132)
            .WithTimeout(TimeSpan.FromSeconds(30))
            .WithProxy("http://proxy:8080", true)
            .WithDebug(true)
            .BuildOptions();

        options.BrowserType.Should().Be(BrowserType.Firefox132);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        options.ProxyUrl.Should().Be("http://proxy:8080");
        options.IsRotatingProxy.Should().BeTrue();
        options.WithDebug.Should().BeTrue();
    }
}