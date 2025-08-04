using Http2Client.Utilities;

using System;
using System.Runtime.InteropServices;

namespace Http2Client.Native;

/// <summary>
/// Wrapper around the native Go TLS library
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
    /// Create wrapper from an already loaded library. Don't call this directly - use Load() instead.
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
    /// Load the native TLS library and create a wrapper. This is what you should call.
    /// If anything goes wrong, we'll clean up and throw an exception.
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
    /// Send an HTTP request through the native library.
    /// </summary>
    public string Request(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _requestDelegate(payload));
    }

    /// <summary>
    /// Get all cookies stored in a session for a specific URL. Useful for debugging.
    /// </summary>
    public string GetCookiesFromSession(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _getCookiesDelegate(payload));
    }

    /// <summary>
    /// Add cookies to a session so they'll be sent automatically on future requests.
    /// </summary>
    public string AddCookiesToSession(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _addCookiesDelegate(payload));
    }

    /// <summary>
    /// Clean up a specific session and free its memory in the native library.
    /// </summary>
    public string DestroySession(byte[] payload)
    {
        ThrowException.Null(payload);
        return ExecuteFunction(() => _destroySessionDelegate(payload));
    }

    /// <summary>
    /// Nuclear option: destroy ALL sessions in the native library.
    /// This will break other Http2Client instances!
    /// </summary>
    public string DestroyAllSessions()
    {
        return ExecuteFunction(() => _destroyAllDelegate());
    }

    /// <summary>
    /// Tell the native library to free memory for a specific response.
    /// Called automatically after each request.
    /// </summary>
    public void FreeMemory(string responseId)
    {
        ThrowException.NullOrEmpty(responseId);
        _freeMemoryDelegate(responseId);
    }

    /// <summary>
    /// Find a function in the loaded library and turn it into a C# delegate we can call.
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
    /// Call a native function and convert its result from a C string to a C# string.
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