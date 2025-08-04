using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Http2Client.Utilities;

/// <summary>
/// Figures out what OS and architecture we're running on. Needed to find the right native library.
/// </summary>
internal static class PlatformSupport
{
    private static readonly PlatformInfo CurrentPlatform = GetCurrentPlatform();

    /// <summary>
    /// Check if we're running on Linux
    /// </summary>
    public static bool IsLinux => CurrentPlatform.Type == OSPlatform.Linux;

    /// <summary>
    /// Check if we're running on macOS
    /// </summary>
    public static bool IsMacOS => CurrentPlatform.Type == OSPlatform.OSX;

    /// <summary>
    /// Check if we're running on Windows
    /// </summary>
    public static bool IsWindows => CurrentPlatform.Type == OSPlatform.Windows;

    /// <summary>
    /// Get the platform name that .NET uses in runtime folders (win, linux, osx).
    /// </summary>
    public static string GetRuntimePlatformName() => CurrentPlatform.Name;

    /// <summary>
    /// Get the file extension for native libraries on this platform (.dll, .so, .dylib).
    /// </summary>
    public static string GetNativeLibraryExtension() => CurrentPlatform.Extension;

    /// <summary>
    /// Get the architecture string for runtime paths. Throws if we don't recognize the architecture.
    /// </summary>
    public static string GetRuntimeArchitecture() => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.X64 => "x64",
        Architecture.X86 => "x86",
        Architecture.Arm => "arm",
        Architecture.Arm64 => "arm64",

        _ => throw new PlatformNotSupportedException($"Architecture {RuntimeInformation.ProcessArchitecture} not supported")
    };

    /// <summary>
    /// Build a standard .NET runtime path like "runtimes/win-x64/tls-client.dll".
    /// This is where NuGet packages put platform-specific files.
    /// </summary>
    public static string GetRuntimePath(string filename)
    {
        return Path.Combine("runtimes", $"{GetRuntimePlatformName()}-{GetRuntimeArchitecture()}", filename);
    }

    private static PlatformInfo GetCurrentPlatform() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? new(OSPlatform.Linux, "linux", "so") :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new(OSPlatform.OSX, "osx", "dylib") :
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new(OSPlatform.Windows, "win", "dll") :
        throw new PlatformNotSupportedException("Unsupported platform - we only support Windows, Linux, and macOS");

    private class PlatformInfo(OSPlatform type, string runtimeName, string extension)
    {
        public OSPlatform Type { get; } = type;
        public string Name { get; } = runtimeName;
        public string Extension { get; } = extension;
    }
}