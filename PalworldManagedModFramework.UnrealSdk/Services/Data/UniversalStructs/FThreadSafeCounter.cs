using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FThreadSafeCounter {
        public int Counter;
    }
}
