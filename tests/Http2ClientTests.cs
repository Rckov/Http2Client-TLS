using FluentAssertions;

using Http2Client.Core.Enums;

namespace Http2Client.Test;

public class Http2ClientTests
{
    [Fact]
    public void Ctor_Null_Throws()
    {
        var action = () => new Http2Client(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_Invalid_Throws()
    {
        var options = new Http2ClientOptions { Timeout = TimeSpan.Zero };

        var action = () => new Http2Client(options);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Ctor_NoLib_Throws()
    {
        var options = new Http2ClientOptions { LibraryPath = "nonexistent.dll" };

        var action = () => new Http2Client(options);

        action.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void SessionId_Works()
    {
        var options = new Http2ClientOptions();
        var expectedSessionId = options.SessionID;

        expectedSessionId.Should().NotBeEmpty();
        options.SessionID.Should().Be(expectedSessionId);
    }

    [Fact]
    public void Options_Works()
    {
        var options = new Http2ClientOptions();

        options.BrowserType.Should().Be(BrowserType.Chrome131);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
    }
}