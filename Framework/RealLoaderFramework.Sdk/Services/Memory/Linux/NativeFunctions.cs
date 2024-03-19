using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory.Linux {
    [SupportedOSPlatform("linux")]
    internal static partial class NativeFunctions {
        [LibraryImport("libc.so.6", EntryPoint = "mmap", SetLastError = true)]
        public static partial nint MemoryMap(nint addr, nuint length, MProtectProtection protect, MMapFlags flags, int fileDescriptor, int offset);

        [LibraryImport("libc.so.6", EntryPoint = "munmap", SetLastError = true)]
        public static partial int MemoryUnmap(nint addr, nuint length);

        [LibraryImport("libc.so.6", EntryPoint = "mprotect", SetLastError = true)]
        public static partial int MemoryProtect(nint addr, nuint length, MProtectProtection protect);

        public static MProtectProtection ToMProtection(this SimpleMemoryProtection memoryProtection) {
            var hasRead = memoryProtection.HasFlag(SimpleMemoryProtection.Read);
            var hasWrite = memoryProtection.HasFlag(SimpleMemoryProtection.Write);
            var hasExecute = memoryProtection.HasFlag(SimpleMemoryProtection.Execute);

            var linuxMProtectFlags = MProtectProtection.PROT_NONE;

            if (hasRead) {
                linuxMProtectFlags |= MProtectProtection.PROT_READ;
            }

            if (hasWrite) {
                linuxMProtectFlags |= MProtectProtection.PROT_WRITE;
            }

            if (hasExecute) {
                linuxMProtectFlags |= MProtectProtection.PROT_EXEC;
            }

            return linuxMProtectFlags;
        }

        public static SimpleMemoryProtection ToSimpleMemoryProtection(this MProtectProtection protection) {
            var hasRead = protection.HasFlag(MProtectProtection.PROT_READ);
            var hasWrite = protection.HasFlag(MProtectProtection.PROT_WRITE);
            var hasExecute = protection.HasFlag(MProtectProtection.PROT_EXEC);

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
        public enum MProtectProtection {
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
