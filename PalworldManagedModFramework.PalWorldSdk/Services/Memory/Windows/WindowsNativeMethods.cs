using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows {
    /// <summary>
    /// Native methods for working in Windows process memory.
    /// </summary>
    public static class WindowsNativeMethods {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint VirtualAlloc(nint baseAddress, uint size,
            WindowsStructs.MemoryAllocationFlags allocationType, WindowsStructs.MemoryProtectionFlags protection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtect(nint address, uint size,
            WindowsStructs.PageProtection newProtect, out WindowsStructs.PageProtection oldProtect);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public unsafe static extern int VirtualQuery(nint hProcess, nint lpAddress,
            WindowsStructs.MEMORY_BASIC_INFORMATION64* lpBuffer, uint dwLength);
    }

    public class WindowsStructs {
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION64 {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public int __alignment1;
            public ulong RegionSize;
            public int State;
            public MemoryProtection Protect;
            public int Type;
            public int __alignment2;
        }

        /// <summary>
        /// Defines memory protection constants for the 'Protect' field in MEMORY_BASIC_INFORMATION64.
        /// </summary>
        [Flags]
        public enum MemoryProtection {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_TARGETS_INVALID = 0x40000000,
            PAGE_TARGETS_NO_UPDATE = 0x40000000
        }




        [Flags]
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

        [Flags]
        public enum MemoryAllocationFlags {
            Commit = 0x01000,
            Reserve = 0x02000
        }

        [Flags]
        public enum MemoryProtectionFlags {
            ExecuteReadWrite = 0x040,
        }
    }
}