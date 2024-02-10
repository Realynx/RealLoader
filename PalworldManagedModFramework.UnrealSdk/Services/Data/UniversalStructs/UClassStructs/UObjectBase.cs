using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public unsafe struct UObjectBase {
        public void* vptrUObjectBase;
        public EObjectFlags objectFlags;
        public uint internalIndex;
        public UClass* classPrivate;
        public FName namePrivate;
        public UObject* outerPrivate;
    }
}
