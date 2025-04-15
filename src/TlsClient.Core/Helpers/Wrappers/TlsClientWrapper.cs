using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core.Helpers.Natives;

namespace TlsClient.Core.Helpers.Wrappers
{
    public class TlsClientWrapper : IDisposable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr RequestDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FreeMemoryDelegate(string sessionID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetCookiesFromSessionDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr AddCookiesToSessionDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr DestroySessionDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr DestroyAllDelegate();

        private readonly IntPtr _module;
        private readonly RequestDelegate _requestDelegate;
        private readonly FreeMemoryDelegate _freeMemoryDelegate;
        private readonly GetCookiesFromSessionDelegate _getCookiesFromSessionDelegate;
        private readonly AddCookiesToSessionDelegate _addCookiesToSessionDelegate;
        private readonly DestroySessionDelegate _destroySessionDelegate;
        private readonly DestroyAllDelegate _destroyAllDelegate;

        public TlsClientWrapper(IntPtr module)
        {
            _module = module;
            _requestDelegate = GetDelegate<RequestDelegate>("request");
            _freeMemoryDelegate = GetDelegate<FreeMemoryDelegate>("freeMemory");
            _getCookiesFromSessionDelegate = GetDelegate<GetCookiesFromSessionDelegate>("getCookiesFromSession");
            _addCookiesToSessionDelegate = GetDelegate<AddCookiesToSessionDelegate>("addCookiesToSession");
            _destroySessionDelegate = GetDelegate<DestroySessionDelegate>("destroySession");
            _destroyAllDelegate = GetDelegate<DestroyAllDelegate>("destroyAll");
        }

        private T GetDelegate<T>(string functionName) where T : Delegate
        {
            IntPtr functionPtr = NativeLoader.GetProcAddress(_module, functionName);
            if (functionPtr == IntPtr.Zero)
            {
                throw new Exception($"Failed to get the address of the '{functionName}' function.");
            }
            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }

        private async Task<string> ExecuteNativeMethodAsync(Func<IntPtr> nativeMethod, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                IntPtr resultPtr = nativeMethod();
                string result = Marshal.PtrToStringAnsi(resultPtr) ?? throw new InvalidOperationException("Received null pointer from native request.");
                return result;
            }, cancellationToken);
        }

        public Task<string> RequestAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => _requestDelegate(payload), cancellationToken);
        }

        public Task FreeMemoryAsync(string sessionID, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                _freeMemoryDelegate(sessionID);
            }, cancellationToken);
        }

        public Task<string> GetCookiesFromSessionAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => _getCookiesFromSessionDelegate(payload), cancellationToken);
        }

        public Task<string> AddCookiesToSessionAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => _addCookiesToSessionDelegate(payload), cancellationToken);
        }

        public Task<string> DestroySessionAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => _destroySessionDelegate(payload), cancellationToken);
        }

        public Task<string> DestroyAllAsync(CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => _destroyAllDelegate(), cancellationToken);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
