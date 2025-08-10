using FluentAssertions;

using Xunit;

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
    public void Ctor_ValidOptions_Works()
    {
        var options = new Http2ClientOptions { LibraryPath = TestConstants.LibraryPath };

        using var client = new Http2Client(options);

        client.Options.Should().BeSameAs(options);
        client.SessionId.Should().Be(options.SessionId);
        client.IsDisposed.Should().BeFalse();
    }

    [Fact]
    public void Dispose_Works()
    {
        var options = new Http2ClientOptions { LibraryPath = TestConstants.LibraryPath };
        var client = new Http2Client(options);

        client.Dispose();

        client.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Send_NullRequest_Throws()
    {
        var options = new Http2ClientOptions { LibraryPath = TestConstants.LibraryPath };
        using var client = new Http2Client(options);

        var action = () => client.Send(null!);
        action.Should().Throw<ArgumentNullException>();
    }
}