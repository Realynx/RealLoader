using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs {
    [StructLayout(LayoutKind.Sequential)]
    public class TArray<Type> {
        public long AllocatorInstance;
        public uint ArrayNum;
        public uint ArrayMax;
    }
}
