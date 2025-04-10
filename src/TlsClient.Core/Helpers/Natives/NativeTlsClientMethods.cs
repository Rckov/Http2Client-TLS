using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TlsClient.Core.Helpers.Natives
{
    public static class NativeTlsClientMethods
    {
        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "request")]
        public static extern IntPtr Request(byte[] payload);

        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "freeMemory")]
        public static extern void FreeMemory(string sessionID);
    }
}
