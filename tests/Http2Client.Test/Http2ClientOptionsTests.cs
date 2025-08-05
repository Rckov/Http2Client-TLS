using FluentAssertions;

using Http2Client.Core.Enums;

namespace Http2Client.Test;

public class Http2ClientOptionsTests
{
    [Fact]
    public void Constructor_SetsDefaults()
    {
        var options = new Http2ClientOptions();

        options.SessionID.Should().NotBeEmpty();
        options.BrowserType.Should().Be(BrowserType.Chrome131);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
        options.CatchPanics.Should().BeTrue();
        options.DefaultHeaders.Should().BeEmpty();
    }

    [Fact]
    public void UserAgent_SetAndGet_UpdatesHeadersCorrectly()
    {
        var options = new Http2ClientOptions();
        const string userAgent = "Test-Agent/1.0";

        options.UserAgent = userAgent;

        options.UserAgent.Should().Be(userAgent);
        options.DefaultHeaders["User-Agent"].Should().ContainSingle().Which.Should().Be(userAgent);
    }

    [Fact]
    public void UserAgent_SetNull_RemovesHeader()
    {
        var options = new Http2ClientOptions();
        options.UserAgent = "Test-Agent/1.0";

        options.UserAgent = null;

        options.UserAgent.Should().BeNull();
        options.DefaultHeaders.Should().NotContainKey("User-Agent");
    }

    [Fact]
    public void Validate_BothCookieOptions_Throws()
    {
        // Create a temporary file to avoid FileNotFoundException
        var tempFile = Path.GetTempFileName();
        try
        {
            var options = new Http2ClientOptions
            {
                LibraryPath = tempFile,
                WithDefaultCookieJar = true,
                WithoutCookieJar = true
            };

            options.Invoking(o => o.Validate())
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot enable both WithDefaultCookieJar and WithoutCookieJar.");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Validate_BothIPDisabled_Throws()
    {
        // Create a temporary file to avoid FileNotFoundException
        var tempFile = Path.GetTempFileName();
        try
        {
            var options = new Http2ClientOptions
            {
                LibraryPath = tempFile,
                DisableIPv4 = true,
                DisableIPv6 = true
            };

            options.Invoking(o => o.Validate())
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot disable both IPv4 and IPv6.");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Validate_ZeroTimeout_Throws()
    {
        // Create a temporary file to avoid FileNotFoundException
        var tempFile = Path.GetTempFileName();
        try
        {
            var options = new Http2ClientOptions
            {
                LibraryPath = tempFile,
                Timeout = TimeSpan.Zero
            };

            options.Invoking(o => o.Validate())
                .Should().Throw<ArgumentException>()
                .WithMessage("Timeout must be greater than zero.*");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Validate_MissingLibraryFile_Throws()
    {
        var options = new Http2ClientOptions { LibraryPath = "nonexistent.dll" };

        options.Invoking(o => o.Validate())
            .Should().Throw<FileNotFoundException>()
            .WithMessage("Native library not found at: nonexistent.dll");
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
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