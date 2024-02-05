using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Linux {
    public static class LinuxNativeMethods {
        [DllImport("libc.so.6", EntryPoint = "gettid")]
        public static extern long GetCurrentThreadId();

        [DllImport("libc.so.6", EntryPoint = "getppid")]
        public static extern long GetParentProcId();

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "ptrace")]
        public static extern long Trace(LinuxStructs.PtraceRequest request, int pid, IntPtr addr, IntPtr data);

        [DllImport("libc.so.6", SetLastError = true, EntryPoint = "waitpid")]
        public static extern int WaitPid(int pid, out int status, int options);

    }

    public class LinuxStructs {
        public enum PtraceRequest {
            PTRACE_TRACEME = 0,
            PTRACE_PEEKTEXT = 1,
            PTRACE_PEEKDATA = 2,
            PTRACE_PEEKUSER = 3,
            PTRACE_POKETEXT = 4,
            PTRACE_POKEDATA = 5,
            PTRACE_POKEUSER = 6,
            PTRACE_CONT = 7,
            PTRACE_KILL = 8,
            PTRACE_SINGLESTEP = 9,
            PTRACE_GETREGS = 12,
            PTRACE_SETREGS = 13,
            PTRACE_GETFPREGS = 14,
            PTRACE_SETFPREGS = 15,
            PTRACE_ATTACH = 16,
            PTRACE_DETACH = 17,
            PTRACE_GETFPXREGS = 18,
            PTRACE_SETFPXREGS = 19,
            PTRACE_SYSCALL = 24,
            PTRACE_SETOPTIONS = 0x4200,
            PTRACE_GETEVENTMSG = 0x4201,
            PTRACE_GETSIGINFO = 0x4202,
            PTRACE_SETSIGINFO = 0x4203
        }

        public enum LinuxError {
            EPERM = 1,           // Operation not permitted
            ENOENT = 2,          // No such file or directory
            ESRCH = 3,           // No such process
            EINTR = 4,           // Interrupted system call
            EIO = 5,             // I/O error
            ENXIO = 6,           // No such device or address
            E2BIG = 7,           // Argument list too long
            ENOEXEC = 8,         // Exec format error
            EBADF = 9,           // Bad file number
            ECHILD = 10,         // No child processes
            EAGAIN = 11,         // Try again
            ENOMEM = 12,         // Out of memory
            EACCES = 13,         // Permission denied
            EFAULT = 14,         // Bad address
            ENOTBLK = 15,        // Block device required
            EBUSY = 16,          // Device or resource busy
            EEXIST = 17,         // File exists
            EXDEV = 18,          // Cross-device link
            ENODEV = 19,         // No such device
            ENOTDIR = 20,        // Not a directory
            EISDIR = 21,         // Is a directory
            EINVAL = 22,         // Invalid argument
            ENFILE = 23,         // File table overflow
            EMFILE = 24,         // Too many open files
            ENOTTY = 25,         // Not a typewriter
            ETXTBSY = 26,        // Text file busy
            EFBIG = 27,          // File too large
            ENOSPC = 28,         // No space left on device
            ESPIPE = 29,         // Illegal seek
            EROFS = 30,          // Read-only file system
            EMLINK = 31,         // Too many links
            EPIPE = 32,          // Broken pipe
            EDOM = 33,           // Math argument out of domain of func
            ERANGE = 34,         // Math result not representable
                                 // Add more as needed
        }
    }
}
