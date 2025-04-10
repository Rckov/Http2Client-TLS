using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core.Helpers.Natives;

namespace TlsClient.Core.Helpers.Wrappers
{
    public static class TlsClientAsyncWrapper
    {
        private static async Task<string> ExecuteNativeMethodAsync(Func<IntPtr> nativeMethod, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                IntPtr resultPtr = nativeMethod();
                string? result = Marshal.PtrToStringAnsi(resultPtr);
                if (result == null)
                {
                    throw new InvalidOperationException("Received null pointer from native request.");
                }
                return result;
            }, cancellationToken);
        }

        public static Task<string> RequestAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => NativeHttpMethods.Request(payload), cancellationToken);
        }

        public static Task FreeMemoryAsync(string sessionID, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                NativeHttpMethods.FreeMemory(sessionID);
            }, cancellationToken);
        }

        public static Task<string> GetCookiesFromSessionAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => NativeHttpMethods.GetCookiesFromSession(payload), cancellationToken);
        }

        public static Task<string> DestroySessionAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => NativeHttpMethods.DestroySession(payload), cancellationToken);
        }

        public static Task<string> DestroyAll(CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(NativeHttpMethods.DestroyAll, cancellationToken);
        }

        public static Task<string> AddCookiesToSessionAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return ExecuteNativeMethodAsync(() => NativeHttpMethods.AddCookiesToSession(payload), cancellationToken);
        }
    }
}
