using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FNameEntry {
        public FNameEntryHeader header;
        public byte StringPointer;
    }
}
