using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Windows {
    /// <summary>
    /// Native methods for working in Windows process memory.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal static partial class NativeFunctions {
        [LibraryImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static unsafe partial int VirtualQuery(nint hProcess, nint lpAddress,
            MEMORY_BASIC_INFORMATION64* lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        public static extern int SuspendThread(Handle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(Handle hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Handle OpenThread(ThreadAccess dwDesiredAccess,  bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(Handle handle);

        [LibraryImport("kernel32.dll")]
        public static partial uint GetCurrentThreadId();

        [LibraryImport("kernel32.dll")]
        public static partial nint VirtualAlloc(nint lpStartAddr, uint size, PageState flAllocationType, MemoryProtection flProtect);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool VirtualFree(nint lpAddress, uint dwSize, PageState dwFreeType);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool VirtualProtect(nint lpAddress, uint dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION64 {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public int __alignment1;
            public ulong RegionSize;
            public PageState State;
            public MemoryProtection Protect;
            public int Type;
            public int __alignment2;
        }

        public enum PageState {
            MEM_COMMIT = 0x1000,
            MEM_RESERVE = 0x2000,
            MEM_RELEASE = 0x8000,
            MEM_FREE = 0x10000,
        }

        public static MemoryProtection ToMemoryProtection(this SimpleMemoryProtection memoryProtection) {
            var hasRead = memoryProtection.HasFlag(SimpleMemoryProtection.Read);
            var hasWrite = memoryProtection.HasFlag(SimpleMemoryProtection.Write);
            var hasExecute = memoryProtection.HasFlag(SimpleMemoryProtection.Execute);

            if (hasRead && hasWrite && hasExecute) {
                return MemoryProtection.PAGE_EXECUTE_READWRITE;
            }

            if (hasRead && hasWrite && !hasExecute) {
                return MemoryProtection.PAGE_READWRITE;
            }

            if (hasRead && !hasWrite && hasExecute) {
                return MemoryProtection.PAGE_EXECUTE_READ;
            }

            if (hasRead && !hasWrite && !hasExecute) {
                return MemoryProtection.PAGE_READONLY;
            }

            if (!hasRead && !hasWrite && hasExecute) {
                return MemoryProtection.PAGE_EXECUTE;
            }

            return MemoryProtection.PAGE_READONLY;
        }

        public static SimpleMemoryProtection ToSimpleMemoryProtection(this MemoryProtection protection) {
            var hasRead = protection is MemoryProtection.PAGE_EXECUTE_READ or MemoryProtection.PAGE_READONLY or MemoryProtection.PAGE_EXECUTE_READWRITE or MemoryProtection.PAGE_READWRITE;
            var hasWrite = protection is MemoryProtection.PAGE_READWRITE or MemoryProtection.PAGE_EXECUTE_READWRITE;
            var hasExecute = protection is MemoryProtection.PAGE_EXECUTE_READWRITE or MemoryProtection.PAGE_EXECUTE or MemoryProtection.PAGE_EXECUTE_READ;

            var simpleMemoryProtection = SimpleMemoryProtection.None;

            if (hasRead) {
                simpleMemoryProtection |= SimpleMemoryProtection.Read;
            }

            if (hasWrite) {
                simpleMemoryProtection |= SimpleMemoryProtection.Write;
            }

            if (hasExecute) {
                simpleMemoryProtection |= SimpleMemoryProtection.Execute;
            }

            return simpleMemoryProtection;
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