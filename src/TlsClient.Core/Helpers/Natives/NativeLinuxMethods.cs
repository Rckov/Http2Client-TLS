using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TlsClient.Core.Helpers.Natives
{
    public static class NativeLinuxMethods
    {
        [Flags]
        public enum LoadLibraryFlags
        {
            None = 0,
            Lazy = 0x0001,
            Now = 0x0002,
            BindingMask = 0x0003,
            NoLoad = 0x0004,
            DeepBind = 0x0008,
            Local = None,
            Global = 0x0100,
            NoDelete = 0x1000
        }

        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        public static extern IntPtr LoadLibrary([In][MarshalAs(UnmanagedType.LPStr)] string path, [In] LoadLibraryFlags flags = LoadLibraryFlags.Now | LoadLibraryFlags.Global);

        [DllImport("libdl.so.2", EntryPoint = "dlclose")]
        public static extern int FreeLibrary([In] IntPtr hLibrary);

    }
}
