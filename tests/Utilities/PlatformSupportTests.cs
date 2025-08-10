using FluentAssertions;

using Http2Client.Utilities;

using Xunit;

namespace Http2Client.Test.Utilities;

public class PlatformSupportTests
{
    [Fact]
    public void GetRuntimePath_BuildsCorrectPath()
    {
        const string filename = "test.dll";

        var path = PlatformSupport.GetRuntimePath(filename);

        path.Should().StartWith("runtimes");
        path.Should().EndWith(filename);
        path.Should().Contain(PlatformSupport.GetRuntimePlatformName());
        path.Should().Contain(PlatformSupport.GetRuntimeArchitecture());
    }

    [Fact]
    public void PlatformFlags_OnlyOneIsTrue()
    {
        var flags = new[] { PlatformSupport.IsWindows, PlatformSupport.IsLinux, PlatformSupport.IsMacOS };

        flags.Count(f => f).Should().Be(1);
    }

    [Fact]
    public void PlatformDetection_ConsistentWithExtension()
    {
        var extension = PlatformSupport.GetNativeLibraryExtension();

        if (PlatformSupport.IsWindows)
        {
            extension.Should().Be("dll");
        }
        else if (PlatformSupport.IsLinux)
        {
            extension.Should().Be("so");
        }
        else if (PlatformSupport.IsMacOS)
        {
            extension.Should().Be("dylib");
        }
    }
}