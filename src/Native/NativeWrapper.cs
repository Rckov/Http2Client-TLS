using Http2Client.Utilities;

using System;
using System.Runtime.InteropServices;

namespace Http2Client.Native;

/// <summary>
/// Wrapper for native Go TLS library.
/// </summary>
internal static class NativeWrapper
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr RequestDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FreeMemoryDelegate(string sessionId);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr GetCookiesFromSessionDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr AddCookiesToSessionDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr DestroySessionDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr DestroyAllDelegate();

    private static IntPtr _libraryHandle;
    private static RequestDelegate? _requestDelegate;
    private static FreeMemoryDelegate? _freeMemoryDelegate;
    private static GetCookiesFromSessionDelegate? _getCookiesDelegate;
    private static AddCookiesToSessionDelegate? _addCookiesDelegate;
    private static DestroySessionDelegate? _destroySessionDelegate;
    private static DestroyAllDelegate? _destroyAllDelegate;

    public static bool IsInitialized { get; private set; }

    /// <summary>
    /// Initializes native library once. Call at application startup.
    /// </summary>
    public static void Initialize(string? path)
    {
        if (IsInitialized)
        {
            return;
        }

        ThrowException.FileNotExists(path);

        _libraryHandle = NativeLoader.LoadLibrary(path);
        try
        {
            // Find all the functions we need in the loaded library
            _requestDelegate = GetDelegate<RequestDelegate>("request");
            _freeMemoryDelegate = GetDelegate<FreeMemoryDelegate>("freeMemory");
            _getCookiesDelegate = GetDelegate<GetCookiesFromSessionDelegate>("getCookiesFromSession");
            _addCookiesDelegate = GetDelegate<AddCookiesToSessionDelegate>("addCookiesToSession");
            _destroySessionDelegate = GetDelegate<DestroySessionDelegate>("destroySession");
            _destroyAllDelegate = GetDelegate<DestroyAllDelegate>("destroyAll");

            IsInitialized = true;
        }
        catch
        {
            // If initialization fails, clean up the loaded library
            NativeLoader.FreeLibrary(_libraryHandle);
            throw;
        }
    }

    /// <summary>
    /// Sends HTTP request through native library.
    /// </summary>
    public static string Request(byte[] payload)
    {
        ThrowIsNotLoaded();
        return ExecuteFunction(() => _requestDelegate!(payload));
    }

    /// <summary>
    /// Gets cookies from session for URL.
    /// </summary>
    public static string GetCookiesFromSession(byte[] payload)
    {
        ThrowIsNotLoaded();
        return ExecuteFunction(() => _getCookiesDelegate!(payload));
    }

    /// <summary>
    /// Adds cookies to session for automatic sending.
    /// </summary>
    public static string AddCookiesToSession(byte[] payload)
    {
        ThrowIsNotLoaded();
        return ExecuteFunction(() => _addCookiesDelegate!(payload));
    }

    /// <summary>
    /// Destroys session and frees memory.
    /// </summary>
    public static string DestroySession(byte[] payload)
    {
        ThrowIsNotLoaded();
        return ExecuteFunction(() => _destroySessionDelegate!(payload));
    }

    /// <summary>
    /// Destroys ALL sessions. Breaks other client instances!
    /// </summary>
    public static string DestroyAllSessions()
    {
        ThrowIsNotLoaded();
        return ExecuteFunction(() => _destroyAllDelegate!());
    }

    /// <summary>
    /// Frees response memory. Called automatically.
    /// </summary>
    public static void FreeMemory(string responseId)
    {
        ThrowIsNotLoaded();
        _freeMemoryDelegate!(responseId);
    }

    /// <summary>
    /// Cleanup native library resources. Call at application shutdown.
    /// </summary>
    public static void Cleanup()
    {
        if (_libraryHandle != IntPtr.Zero)
        {
            NativeLoader.FreeLibrary(_libraryHandle);
        }

        _libraryHandle = IntPtr.Zero;

        // Clear delegates
        _requestDelegate = null;
        _freeMemoryDelegate = null;
        _getCookiesDelegate = null;
        _addCookiesDelegate = null;
        _destroySessionDelegate = null;
        _destroyAllDelegate = null;

        // 
        IsInitialized = false;
    }

    /// <summary>
    /// Ensures library is initialized before use.
    /// </summary>
    private static void ThrowIsNotLoaded()
    {
        if (IsInitialized)
        {
            return;
        }

        throw new InvalidOperationException("Native library is not initialized. Call Http2Client.Initialize(libraryPath) before creating any Http2Client instances.");
    }

    /// <summary>
    /// Finds function in library and creates delegate.
    /// </summary>
    private static T GetDelegate<T>(string functionName)
    {
        var functionPtr = NativeLoader.GetProcAddress(_libraryHandle, functionName);

        if (functionPtr == IntPtr.Zero)
        {
            throw new EntryPointNotFoundException($"Function '{functionName}' not found in native library. Wrong library version?");
        }

        return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
    }

    /// <summary>
    /// Calls native function and converts result to string.
    /// </summary>
    private static string ExecuteFunction(Func<IntPtr> nativeFunction)
    {
        var resultPtr = nativeFunction();

        if (resultPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Native function returned null. The library might have crashed or run out of memory.");
        }

        return Marshal.PtrToStringAnsi(resultPtr)!;
    }
}