using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Services.Detour.Windows {
    public static class NativeFunctions {
        [DllImport("kernel32.dll")]
        public static extern uint VirtualAlloc(nint lpStartAddr, uint size, StateEnum flAllocationType, Protection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree(nint lpAddress, uint dwSize, StateEnum dwFreeType);


        public enum StateEnum {
            MEM_COMMIT = 0x1000,
            MEM_RESERVE = 0x2000,
            MEM_RELEASE = 0x8000,
            MEM_FREE = 0x10000,
        }

        public enum Protection {
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
        }
    }
}
