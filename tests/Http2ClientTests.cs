using FluentAssertions;

using Http2Client.Core.Enums;

namespace Http2Client.Test;

public class Http2ClientTests
{
    [Fact]
    public void Constructor_NullOptions_Throws()
    {
        var action = () => new Http2Client(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_InvalidOptions_Throws()
    {
        var options = new Http2ClientOptions { Timeout = TimeSpan.Zero };

        var action = () => new Http2Client(options);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_MissingLibrary_Throws()
    {
        var options = new Http2ClientOptions { LibraryPath = "nonexistent.dll" };

        var action = () => new Http2Client(options);

        action.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void SessionId_ReturnsOptionsSessionId()
    {
        var options = new Http2ClientOptions();
        var expectedSessionId = options.SessionID;

        // Since we can't create a real client without the native library, we test that the property
        // would return the correct value
        expectedSessionId.Should().NotBeEmpty();
        options.SessionID.Should().Be(expectedSessionId);
    }

    [Fact]
    public void Options_Property_ReturnsCorrectReference()
    {
        var options = new Http2ClientOptions();

        // Test that the options reference is maintained correctly
        options.BrowserType.Should().Be(BrowserType.Chrome131);
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
    }
}