using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs {
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct UScriptStruct {
        [FieldOffset(0xb0)]
        public EStructFlags structFlags;

        [FieldOffset(0xb4)]
        public bool bPrepareCppStructOpsCompleted;

        [FieldOffset(0xb8)]
        public ICppStructOps* cppStructOps;
    }
}
