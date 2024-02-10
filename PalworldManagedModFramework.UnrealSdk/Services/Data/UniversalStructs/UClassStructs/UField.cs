using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Size = 0x30)]
    public unsafe struct UField {
        public UObjectBase baseUObject;
        public UField* next;
    }
}
