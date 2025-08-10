using FluentAssertions;

using Http2Client.Core.Enums;

using Xunit;

namespace Http2Client.Test;

public class Http2ClientOptionsTests
{
    [Fact]
    public void SetsDefaults()
    {
        var options = new Http2ClientOptions();

        options.SessionId.Should().NotBeEmpty();
        options.BrowserType.Should().Be(BrowserType.Chrome133);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        options.CatchPanics.Should().BeTrue();
        options.DefaultHeaders.Should().BeEmpty();
    }

    [Fact]
    public void UserAgent_Sets()
    {
        var options = new Http2ClientOptions();
        const string userAgent = "Test-Agent/1.0";

        options.UserAgent = userAgent;

        options.UserAgent.Should().Be(userAgent);
        options.DefaultHeaders["User-Agent"].Should().ContainSingle().Which.Should().Be(userAgent);
    }

    [Fact]
    public void UserAgent_Null_Removes()
    {
        var options = new Http2ClientOptions
        {
            UserAgent = "Test-Agent/1.0"
        };

        options.UserAgent = null;

        options.UserAgent.Should().BeNull();
        options.DefaultHeaders.Should().NotContainKey("User-Agent");
    }

    [Fact]
    public void Validate_Cookies_Throws()
    {
        var options = new Http2ClientOptions
        {
            LibraryPath = TestConstants.LibraryPath,
            WithDefaultCookieJar = true,
            WithoutCookieJar = true
        };

        options.Invoking(o => o.Validate())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Cannot enable both WithDefaultCookieJar and WithoutCookieJar.");
    }

    [Fact]
    public void Validate_IP_Throws()
    {
        var options = new Http2ClientOptions
        {
            LibraryPath = TestConstants.LibraryPath,
            DisableIPv4 = true,
            DisableIPv6 = true
        };

        options.Invoking(o => o.Validate())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Cannot disable both IPv4 and IPv6.");
    }

    [Fact]
    public void Validate_Timeout_Throws()
    {
        var options = new Http2ClientOptions
        {
            LibraryPath = TestConstants.LibraryPath,
            Timeout = TimeSpan.Zero
        };

        options.Invoking(o => o.Validate()).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_ProxyUrl_Throws()
    {
        var options = new Http2ClientOptions
        {
            LibraryPath = TestConstants.LibraryPath,
            ProxyUrl = "invalid-url"
        };

        options.Invoking(o => o.Validate()).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_LibraryPath_Throws()
    {
        var options = new Http2ClientOptions { LibraryPath = "nonexistent.dll" };
        options.Invoking(o => o.Validate()).Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void Clone_Works()
    {
        var original = new Http2ClientOptions
        {
            BrowserType = BrowserType.Firefox132,
            Timeout = TimeSpan.FromSeconds(30),
            UserAgent = "Test-Agent/1.0"
        };
        original.DefaultHeaders["Custom-Header"] = ["value1", "value2"];

        var clone = original.Clone();

        clone.Should().NotBeSameAs(original);
        clone.BrowserType.Should().Be(original.BrowserType);
        clone.Timeout.Should().Be(original.Timeout);
        clone.UserAgent.Should().Be(original.UserAgent);
        clone.DefaultHeaders.Should().NotBeSameAs(original.DefaultHeaders);
        clone.DefaultHeaders["Custom-Header"].Should().NotBeSameAs(original.DefaultHeaders["Custom-Header"]);
        clone.DefaultHeaders["Custom-Header"].Should().BeEquivalentTo(["value1", "value2"]);
    }
}