using Http2Client.Utilities;

using System;
using System.Runtime.InteropServices;

namespace Http2Client.Native;

/// <summary>
/// Wrapper for native Go TLS library.
/// </summary>
internal class NativeWrapper : IDisposable
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint RequestDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FreeMemoryDelegate(string sessionID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint GetCookiesFromSessionDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint AddCookiesToSessionDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint DestroySessionDelegate(byte[] payload);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate nint DestroyAllDelegate();

    private readonly nint _libraryHandle;
    private readonly RequestDelegate _requestDelegate;
    private readonly FreeMemoryDelegate _freeMemoryDelegate;
    private readonly GetCookiesFromSessionDelegate _getCookiesDelegate;
    private readonly AddCookiesToSessionDelegate _addCookiesDelegate;
    private readonly DestroySessionDelegate _destroySessionDelegate;
    private readonly DestroyAllDelegate _destroyAllDelegate;

    /// <summary>
    /// Creates wrapper from loaded library. Use Load() instead.
    /// </summary>
    public NativeWrapper(nint libraryHandle)
    {
        _libraryHandle = libraryHandle;

        // Find all the functions we need in the loaded library
        _requestDelegate = GetDelegate<RequestDelegate>("request");
        _freeMemoryDelegate = GetDelegate<FreeMemoryDelegate>("freeMemory");
        _getCookiesDelegate = GetDelegate<GetCookiesFromSessionDelegate>("getCookiesFromSession");
        _addCookiesDelegate = GetDelegate<AddCookiesToSessionDelegate>("addCookiesToSession");
        _destroySessionDelegate = GetDelegate<DestroySessionDelegate>("destroySession");
        _destroyAllDelegate = GetDelegate<DestroyAllDelegate>("destroyAll");
    }

    /// <summary>
    /// Loads native library and creates wrapper. Cleans up on failure.
    /// </summary>
    public static NativeWrapper Load(string path)
    {
        ThrowException.FileNotExists(path);

        var handleLib = NativeLoader.LoadLibrary(path!);
        try
        {
            return new NativeWrapper(handleLib);
        }
        catch
        {
            // If wrapper creation fails, clean up the loaded library
            NativeLoader.FreeLibrary(handleLib);
            throw;
        }
    }

    /// <summary>
    /// Sends HTTP request through native library.
    /// </summary>
    public string Request(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _requestDelegate(payload));
    }

    /// <summary>
    /// Gets cookies from session for URL.
    /// </summary>
    public string GetCookiesFromSession(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _getCookiesDelegate(payload));
    }

    /// <summary>
    /// Adds cookies to session for automatic sending.
    /// </summary>
    public string AddCookiesToSession(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _addCookiesDelegate(payload));
    }

    /// <summary>
    /// Destroys session and frees memory.
    /// </summary>
    public string DestroySession(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _destroySessionDelegate(payload));
    }

    /// <summary>
    /// Destroys ALL sessions. Breaks other client instances!
    /// </summary>
    public string DestroyAllSessions()
    {
        return ExecuteFunction(() => _destroyAllDelegate());
    }

    /// <summary>
    /// Frees response memory. Called automatically.
    /// </summary>
    public void FreeMemory(string responseId)
    {
        ThrowException.NullOrEmpty(responseId);
        _freeMemoryDelegate(responseId);
    }

    /// <summary>
    /// Finds function in library and creates delegate.
    /// </summary>
    private T GetDelegate<T>(string functionName)
    {
        nint functionPtr = NativeLoader.GetProcAddress(_libraryHandle, functionName);

        if (functionPtr == IntPtr.Zero)
        {
            throw new EntryPointNotFoundException($"Function '{functionName}' not found in native library. Wrong library version?");
        }

        return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
    }

    /// <summary>
    /// Calls native function and converts result to string.
    /// </summary>
    private static string ExecuteFunction(Func<nint> nativeFunction)
    {
        var resultPtr = nativeFunction();

        if (resultPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Native function returned null. The library might have crashed or run out of memory.");
        }

        return Marshal.PtrToStringAnsi(resultPtr)!;
    }

    public void Dispose()
    {
        NativeLoader.FreeLibrary(_libraryHandle);
    }
}