using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject {
    [StructLayout(LayoutKind.Sequential)]
    public class TArray<Type> {
        public long AllocatorInstance;
        public uint ArrayNum;
        public uint ArrayMax;
    }
}
