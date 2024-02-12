using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FGuid {
        public uint a;
        public uint b;
        public uint c;
        public uint d;
    }
}
