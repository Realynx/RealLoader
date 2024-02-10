using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FNameEntryId {
        public ushort lowerOrderValue;
        public ushort higherOrderValue;
    }
}
