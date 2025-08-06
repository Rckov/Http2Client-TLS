using FluentAssertions;

using Http2Client.Builders;
using Http2Client.Core.Enums;

namespace Http2Client.Test.Builders;

public class HttpClientBuilderTests
{
    [Fact]
    public void Timeout_Throws()
    {
        var builder = new HttpClientBuilder();

        builder.Invoking(b => b.WithTimeout(TimeSpan.Zero))
            .Should().Throw<ArgumentException>()
            .WithMessage("Timeout must be positive*");
    }

    [Fact]
    public void Cookies_Work()
    {
        var tempFile1 = Path.GetTempFileName();
        var tempFile2 = Path.GetTempFileName();
        try
        {
            var enabledOptions = new HttpClientBuilder()
                .WithLibraryPath(tempFile1)
                .WithCookies(true)
                .BuildOptions();

            var disabledOptions = new HttpClientBuilder()
                .WithLibraryPath(tempFile2)
                .WithCookies(false)
                .BuildOptions();

            enabledOptions.WithDefaultCookieJar.Should().BeTrue();
            enabledOptions.WithoutCookieJar.Should().BeFalse();

            disabledOptions.WithDefaultCookieJar.Should().BeFalse();
            disabledOptions.WithoutCookieJar.Should().BeTrue();
        }
        finally
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }

    [Fact]
    public void UserAgent_Works()
    {
        const string userAgent = "Test-Agent/1.0";
        var tempFile = Path.GetTempFileName();
        try
        {
            var options = new HttpClientBuilder()
                .WithLibraryPath(tempFile)
                .WithUserAgent(userAgent)
                .BuildOptions();

            options.UserAgent.Should().Be(userAgent);
            options.DefaultHeaders["User-Agent"].Should().ContainSingle().Which.Should().Be(userAgent);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Headers_Work()
    {
        var headers = new Dictionary<string, string>
        {
            ["Header1"] = "Value1",
            ["Header2"] = "Value2"
        };
        var tempFile = Path.GetTempFileName();
        try
        {
            var options = new HttpClientBuilder()
                .WithLibraryPath(tempFile)
                .WithHeaders(headers)
                .BuildOptions();

            options.DefaultHeaders["Header1"].Should().ContainSingle().Which.Should().Be("Value1");
            options.DefaultHeaders["Header2"].Should().ContainSingle().Which.Should().Be("Value2");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Chaining_Works()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var options = new HttpClientBuilder()
                .WithLibraryPath(tempFile)
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
        finally
        {
            File.Delete(tempFile);
        }
    }
}