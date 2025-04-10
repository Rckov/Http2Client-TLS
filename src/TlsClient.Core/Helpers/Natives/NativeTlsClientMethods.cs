using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TlsClient.Core.Helpers.Natives
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/7a71edbf6e05acd4ade8e910e4c29c968003e27b/cffi_dist/main.go#L25
    public static class NativeHttpMethods
    {
        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "request")]
        public static extern IntPtr Request(byte[] payload);

        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "freeMemory")]
        public static extern void FreeMemory(string sessionID);

        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "getCookiesFromSession")]
        public static extern IntPtr GetCookiesFromSession(byte[] payload);

        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "destroySession")]
        public static extern IntPtr DestroySession(byte[] payload);

        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "destroyAll")]
        public static extern IntPtr DestroyAll();
        [DllImport("tls-client-latest", CallingConvention = CallingConvention.Cdecl, EntryPoint = "addCookiesToSession")]
        public static extern IntPtr AddCookiesToSession(byte[] payload);
    }
}
