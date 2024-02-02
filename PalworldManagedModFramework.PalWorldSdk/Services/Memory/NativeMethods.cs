using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    /// <summary>
    /// Native methods for working in process memory.
    /// </summary>
    public static class NativeMethods {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();


        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr VirtualAlloc(IntPtr baseAddress, uint size, MemoryAllocationFlags allocationType, MemoryProtectionFlags protection);

        [Flags]
        internal enum MemoryAllocationFlags {
            Commit = 0x01000,
            Reserve = 0x02000
        }

        [Flags]
        internal enum MemoryProtectionFlags {
            ExecuteReadWrite = 0x040,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtect(IntPtr address, uint size, PageProtection newProtect, out PageProtection oldProtect);

        public enum PageProtection : uint {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }
    }
}