using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TlsClient.Core.Helpers.Natives;

namespace TlsClient.Core.Helpers
{
    public static class TlsClientAsyncWrapper
    {
        public static async Task<string> RequestAsync(byte[] payload, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                IntPtr resultPtr = NativeTlsClientMethods.Request(payload);
                string? result = Marshal.PtrToStringAnsi(resultPtr);
                if (result == null)
                {
                    throw new InvalidOperationException("Received null pointer from native request.");
                }
                return result;
            }, cancellationToken);
        }

        public static Task FreeMemoryAsync(string sessionID, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                NativeTlsClientMethods.FreeMemory(sessionID);
            }, cancellationToken);
        }
    }
}
