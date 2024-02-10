using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FNameEntry {
        public FNameEntryHeader header;
        public byte StringPointer;
    }
}
