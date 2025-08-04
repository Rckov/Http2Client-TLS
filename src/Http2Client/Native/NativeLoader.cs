using Http2Client.Utilities;

using System;
using System.Runtime.InteropServices;

namespace Http2Client.Native;

/// <summary>
/// Cross-platform native library loader. Handles the messy details of loading .dll/.so/.dylib files
/// and finding functions inside them. Each OS does this differently, so we wrap it all up here.
/// </summary>
internal static class NativeLoader
{
    /// <summary>
    /// Tell dlopen to resolve all symbols immediately instead of lazily
    /// </summary>
    private const int RTLD_NOW = 2;

    /// <summary>
    /// Load a native library file and get a handle you can use to call functions.
    /// Throws PlatformNotSupportedException if we don't know how to load libraries on this OS.
    /// </summary>
    public static nint LoadLibrary(string path)
    {
        if (PlatformSupport.IsLinux) return Linux.dlopen(path, RTLD_NOW);
        if (PlatformSupport.IsMacOS) return MacOS.dlopen(path, RTLD_NOW);
        if (PlatformSupport.IsWindows) return Windows.LoadLibrary(path);

        throw new PlatformNotSupportedException("Don't know how to load libraries on this platform");
    }

    /// <summary>
    /// Find a function inside a loaded library by name. Like GetProcAddress on Windows.
    /// </summary>
    /// <param name="handle">Library handle from LoadLibrary</param>
    /// <param name="name">Name of the function to find</param>
    public static nint GetProcAddress(nint handle, string name)
    {
        if (PlatformSupport.IsLinux) return Linux.dlsym(handle, name);
        if (PlatformSupport.IsMacOS) return MacOS.dlsym(handle, name);
        if (PlatformSupport.IsWindows) return Windows.GetProcAddress(handle, name);

        throw new PlatformNotSupportedException("Don't know how to find functions on this platform");
    }

    /// <summary>
    /// Unload a library and free its memory. Always call this when you're done.
    /// </summary>
    /// <param name="handle">Library handle from LoadLibrary</param>
    public static bool FreeLibrary(nint handle)
    {
        // Linux and macOS return 0 on success, non-zero on failure
        if (PlatformSupport.IsLinux) return Linux.dlclose(handle) == 0;
        if (PlatformSupport.IsMacOS) return MacOS.dlclose(handle) == 0;
        if (PlatformSupport.IsWindows) return Windows.FreeLibrary(handle);

        throw new PlatformNotSupportedException("Don't know how to unload libraries on this platform");
    }

    /// <summary>
    /// Windows native library functions. Good old kernel32.dll doing the heavy lifting.
    /// </summary>
    private static class Windows
    {
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "LoadLibraryW")]
        public static extern nint LoadLibrary(string path);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GetProcAddress(nint handle, string name);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(nint handle);
    }

    /// <summary>
    /// Linux dynamic loading functions. Uses libdl which should be available everywhere.
    /// </summary>
    private static class Linux
    {
        [DllImport("libdl.so.2", SetLastError = true)]
        public static extern nint dlopen(string path, int flags);

        [DllImport("libdl.so.2", SetLastError = true)]
        public static extern nint dlsym(nint handle, string name);

        [DllImport("libdl.so.2", SetLastError = true)]
        public static extern int dlclose(nint handle);
    }

    /// <summary>
    /// macOS dynamic loading. Same as Linux but uses .dylib instead of .so files.
    /// </summary>
    private static class MacOS
    {
        [DllImport("libdl.dylib", SetLastError = true)]
        public static extern nint dlopen(string path, int flags);

        [DllImport("libdl.dylib", SetLastError = true)]
        public static extern nint dlsym(nint handle, string name);

        [DllImport("libdl.dylib", SetLastError = true)]
        public static extern int dlclose(nint handle);
    }
}