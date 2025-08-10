using Http2Client.Utilities;

using System;
using System.Runtime.InteropServices;

namespace Http2Client.Native;

/// <summary>
/// Cross-platform native library loader. Handles OS-specific loading of .dll/.so/.dylib files.
/// </summary>
internal static class NativeLoader
{
    /// <summary>
    /// Loads native library and returns handle for function calls.
    /// </summary>
    public static IntPtr LoadLibrary(string? path)
    {
        if (PlatformSupport.IsLinux) return Linux.LoadLibrary(path);
        if (PlatformSupport.IsMacOS) return MacOS.LoadLibrary(path);
        if (PlatformSupport.IsWindows) return Windows.LoadLibrary(path);

        throw new PlatformNotSupportedException("Don't know how to load libraries on this platform");
    }

    /// <summary>
    /// Finds function in loaded library by name.
    /// </summary>
    /// <param name="handle">Library handle</param>
    /// <param name="name">Function name</param>
    public static IntPtr GetProcAddress(IntPtr handle, string name)
    {
        if (PlatformSupport.IsLinux) return Linux.GetProcAddress(handle, name);
        if (PlatformSupport.IsMacOS) return MacOS.GetProcAddress(handle, name);
        if (PlatformSupport.IsWindows) return Windows.GetProcAddress(handle, name);

        throw new PlatformNotSupportedException("Don't know how to find functions on this platform");
    }

    /// <summary>
    /// Unloads library and frees memory.
    /// </summary>
    /// <param name="handle">Library handle</param>
    public static bool FreeLibrary(IntPtr handle)
    {
        // Linux and macOS return 0 on success, non-zero on failure
        if (PlatformSupport.IsLinux) return Linux.FreeLibrary(handle) == 0;
        if (PlatformSupport.IsMacOS) return MacOS.FreeLibrary(handle) == 0;
        if (PlatformSupport.IsWindows) return Windows.FreeLibrary(handle);

        throw new PlatformNotSupportedException("Don't know how to unload libraries on this platform");
    }

    /// <summary>
    /// Windows native library functions.
    /// </summary>
    private static class Windows
    {
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "LoadLibraryW")]
        public static extern IntPtr LoadLibrary(
            [In][MarshalAs(UnmanagedType.LPWStr)] string? path);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr hModule,
            [In] string procName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(
            [In] IntPtr hLibrary);
    }

    /// <summary>
    /// Linux dynamic loading functions.
    /// </summary>
    private static class Linux
    {
        [Flags]
        public enum LoadLibraryFlags
        {
            Now = 0x0002,
            Global = 0x0100
        }

        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        public static extern IntPtr LoadLibrary(
            [In][MarshalAs(UnmanagedType.LPStr)] string? path,
            [In] LoadLibraryFlags flags = LoadLibraryFlags.Now | LoadLibraryFlags.Global);

        [DllImport("libdl.so.2", EntryPoint = "dlsym")]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr handle,
            [In] string symbol);

        [DllImport("libdl.so.2", EntryPoint = "dlclose")]
        public static extern int FreeLibrary(
            [In] IntPtr hLibrary);
    }

    /// <summary>
    /// macOS dynamic loading functions.
    /// </summary>
    private static class MacOS
    {
        [Flags]
        public enum LoadLibraryFlags
        {
            Now = 0x02,
            Global = 0x08
        }

        [DllImport("libdl.dylib", EntryPoint = "dlopen")]
        public static extern IntPtr LoadLibrary(
            [In][MarshalAs(UnmanagedType.LPStr)] string? path,
            [In] LoadLibraryFlags flags = LoadLibraryFlags.Now | LoadLibraryFlags.Global);

        [DllImport("libdl.dylib", EntryPoint = "dlsym")]
        public static extern IntPtr GetProcAddress(
            [In] IntPtr handle,
            [In] string symbol);

        [DllImport("libdl.dylib", EntryPoint = "dlclose")]
        public static extern int FreeLibrary(
            [In] IntPtr hLibrary);
    }
}