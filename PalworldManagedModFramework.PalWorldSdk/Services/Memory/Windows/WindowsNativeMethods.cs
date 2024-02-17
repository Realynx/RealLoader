using System.Runtime.InteropServices;

using static PalworldManagedModFramework.Sdk.Services.Memory.Windows.WindowsStructs;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
    /// <summary>
    /// Native methods for working in Windows process memory.
    /// </summary>
    public static class WindowsNativeMethods {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint VirtualAlloc(nint baseAddress, uint size,
            MemoryProtection allocationType, MemoryProtection protection);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtect(nint address, uint size,
            MemoryProtection newProtect, out MemoryProtection oldProtect);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public unsafe static extern int VirtualQuery(nint hProcess, nint lpAddress,
            MEMORY_BASIC_INFORMATION64* lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
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
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
            PAGE_TARGETS_INVALID = 0x40000000,
            PAGE_TARGETS_NO_UPDATE = 0x40000000
        }

        [Flags]
        public enum ThreadAccess {
            TERMINATE = 0x0001,
            SUSPEND_RESUME = 0x0002,
            GET_CONTEXT = 0x0008,
            SET_CONTEXT = 0x0010,
            SET_INFORMATION = 0x0020,
            QUERY_INFORMATION = 0x0040,
            SET_THREAD_TOKEN = 0x0080,
            IMPERSONATE = 0x0100,
            DIRECT_IMPERSONATION = 0x0200
        }
    }
}