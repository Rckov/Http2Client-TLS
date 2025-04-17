using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TlsClient.Core.Helpers.Natives;

namespace TlsClient.Core.Helpers
{
    public class NativeLoader
    {
        public static IntPtr LoadNativeAssembly()
        {
            string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
                              RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                              RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" :
                              throw new PlatformNotSupportedException("Unsupported OS platform");

            string extension = platform switch
            {
                "win" => "dll",
                "linux" => "so",
                "darwin" => "dylib",
                _ => throw new PlatformNotSupportedException("Unsupported OS platform")
            };

            string architecture = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => throw new PlatformNotSupportedException("Unsupported process architecture")
            };

            if (platform == "linux")
            {
                string distro = NativeLinuxMethods.GetLinuxDistro();

                architecture = architecture switch
                {
                    "x64" => "amd64",
                    "x86" => "i386",
                    "arm" => "armhf",
                    "arm64" => "aarch64",
                    _ => architecture
                };

                if (!distro.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase))
                {
                    platform = $"{platform}-{distro}";
                    architecture = architecture.Replace("x", string.Empty);
                }
            }

            string libraryPath = Path.GetFullPath($"runtimes/tls-client/{platform}/{architecture}/tls-client.{extension}");

            if (!File.Exists(libraryPath))
            {
                throw new DllNotFoundException($"The native library '{libraryPath}' was not found.");
            }

            if (platform == "win")
            {
                return NativeWindowsMethods.LoadLibrary(libraryPath);
            }
            else if (platform == "linux" || platform == "linux-ubuntu" || platform == "linux-alpine")
            {
                return NativeLinuxMethods.LoadLibrary(libraryPath);
            }
            else if (platform == "darwin")
            {
                return NativeDarwinMethods.LoadLibrary(libraryPath);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform");
            }

        }

        public static bool FreeNativeAssembly(IntPtr libraryHandle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return NativeWindowsMethods.FreeLibrary(libraryHandle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return NativeLinuxMethods.FreeLibrary(libraryHandle) == 0;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return NativeDarwinMethods.FreeLibrary(libraryHandle) == 0;
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform");
            }
        }

        public static IntPtr GetProcAddress(IntPtr handle, string name) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return NativeWindowsMethods.GetProcAddress(handle, name);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return NativeLinuxMethods.GetProcAddress(handle, name);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return NativeDarwinMethods.GetProcAddress(handle, name);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform");
            }
        }
    }
}
