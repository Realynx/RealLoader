using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Linux {
    [SupportedOSPlatform("linux")]
    public static class NativeFunctions {
        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "mmap")]
        public static extern nint MemoryMap(nint addr, nuint length, MProtectProtect protect, MMapFlags flags, int fileDescriptor, int offset);

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "munmap")]
        public static extern int MemoryUnmap(nint addr, nuint length);

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "mprotect")]
        public static extern int MemoryProtect(nint addr, nuint length, MProtectProtect protect);

        public static MProtectProtect ConvertToMProtection(SimpleMemoryProtection memoryProtection) {
            var hasRead = memoryProtection.HasFlag(SimpleMemoryProtection.Read);
            var hasWrite = memoryProtection.HasFlag(SimpleMemoryProtection.Write);
            var hasExecute = memoryProtection.HasFlag(SimpleMemoryProtection.Execute);

            var linuxMProtectFlags = MProtectProtect.PROT_NONE;

            if (hasRead) {
                linuxMProtectFlags |= MProtectProtect.PROT_READ;
            }

            if (hasWrite) {
                linuxMProtectFlags |= MProtectProtect.PROT_WRITE;
            }

            if (hasExecute) {
                linuxMProtectFlags |= MProtectProtect.PROT_EXEC;
            }

            return linuxMProtectFlags;
        }

        public static SimpleMemoryProtection ConvertToMemoryProtection(MProtectProtect protection) {
            var hasRead = protection.HasFlag(MProtectProtect.PROT_READ);
            var hasWrite = protection.HasFlag(MProtectProtect.PROT_WRITE);
            var hasExecute = protection.HasFlag(MProtectProtect.PROT_EXEC);

            var genericMemoryProtection = SimpleMemoryProtection.None;
            if (hasRead) {
                genericMemoryProtection |= SimpleMemoryProtection.Read;
            }

            if (hasWrite) {
                genericMemoryProtection |= SimpleMemoryProtection.Write;
            }

            if (hasExecute) {
                genericMemoryProtection |= SimpleMemoryProtection.Execute;
            }

            return genericMemoryProtection;
        }

        [Flags]
        public enum MProtectProtect {
            PROT_NONE = 0,  // The memory cannot be accessed at all.
            PROT_READ = 1 << 0,  // The memory can be read.
            PROT_WRITE = 1 << 1, // The memory can be modified.
            PROT_EXEC = 1 << 2,  // The memory can be executed.
        }

        [Flags]
        public enum MMapFlags : ulong {
            MAP_SHARED = 1 << 0,
            MAP_SHARED_VALIDATE = 1 << 1,
            MAP_PRIVATE = 1 << 2,
            MAP_32BIT = 1 << 3,
            MAP_ANON = 1 << 4,
            MAP_ANONYMOUS = 1 << 5,
            MAP_DENYWRITE = 1 << 6,
            MAP_EXECUTABLE = 1 << 7,
            MAP_FILE = 1 << 8,
            MAP_FIXED = 1 << 9,
            MAP_FIXED_NOREPLACE = 1 << 10,
            MAP_GROWSDOWN = 1 << 11,
            MAP_HUGETLB = 1 << 12,
            MAP_HUGE_2MB = 1 << 13,
            MAP_HUGE_1GB = 1 << 14,
            MAP_LOCKED = 1 << 15,
            MAP_NONBLOCK = 1 << 16,
            MAP_NORESERVE = 1 << 17,
            MAP_POPULATE = 1 << 18,
            MAP_STACK = 1 << 19,
            MAP_SYNC = 1 << 20,
            MAP_UNINITIALIZED = 1 << 21,
        }

        public enum MMapErrors {
            EACCES,
            EAGAIN,
            EBADF,
            EEXIST,
            EINVAL,
            EINVAL2,
            EINVAL3,
            ENFILE,
            ENODEV,
            ENOMEM,
            ENOMEM2,
            ENOMEM3,
            EOVERFLOW,
            EPERM,
            EPERM2,
            ETXTBSY,
            SIGSEGV,
            SIGBUS,
        }
    }
}
