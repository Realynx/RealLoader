using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FThreadSafeCounter {
        public int Counter;
    }
}
