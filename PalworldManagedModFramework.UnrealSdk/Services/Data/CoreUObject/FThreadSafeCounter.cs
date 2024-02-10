using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FThreadSafeCounter {
        public int Counter;
    }
}
