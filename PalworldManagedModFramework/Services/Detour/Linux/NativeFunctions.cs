using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Services.Detour.Linux {
    public static class NativeFunctions {

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "mmap")]
        public static extern IntPtr MemoryMap(nint addr, nuint length, MProtectProtect protect, MMapFlags flags, int fileDescriptor, int offset);

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "munmap")]
        public static extern int MemoryUnmap(nint addr, nuint length);

        [Flags]
        public enum MProtectProtect {
            PROT_NONE = 0,  // The memory cannot be accessed at all.
            PROT_READ = 1 << 0,  // The memory can be read.
            PROT_WRITE = 1 << 1, // The memory can be modified.
            PROT_EXEC = 1 << 2,  // The memory can be executed.
        }

        [Flags]
        public enum MMapFlags {
            MAP_SHARED,
            MAP_SHARED_VALIDATE,
            MAP_PRIVATE,
            MAP_32BIT,
            [Obsolete]
            MAP_ANON,
            MAP_ANONYMOUS,
            MAP_DENYWRITE,
            MAP_EXECUTABLE,
            MAP_FILE,
            MAP_FIXED,
            MAP_FIXED_NOREPLACE,
            MAP_GROWSDOWN,
            MAP_HUGETLB,
            MAP_HUGE_2MB,
            MAP_HUGE_1GB,
            MAP_LOCKED,
            MAP_NONBLOCK,
            MAP_NORESERVE,
            MAP_POPULATE,
            MAP_STACK,
            MAP_SYNC,
            MAP_UNINITIALIZED,
        }
    }
}
