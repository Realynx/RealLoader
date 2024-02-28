using System.Runtime.InteropServices;

namespace RealLoaderFramework.Sdk.Models.Windows {
    [StructLayout(LayoutKind.Sequential)]
    public struct FWindowsCriticalSection {
        public CRITICAL_SECTION criticalSection;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CRITICAL_SECTION {
        public _RTL_CRITICAL_SECTION_DEBUG* DebugInfo;
        public long LockCount;
        public long RecursionCount;

        public void* OwningThread;
        public void* LockSemaphore;

        public ulong SpinCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class _RTL_CRITICAL_SECTION_DEBUG {
        public ushort Type;
        // More to add if needed...
    }
}
