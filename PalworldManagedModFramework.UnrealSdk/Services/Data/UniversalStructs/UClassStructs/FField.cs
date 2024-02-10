using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct FField {
        public void* vptrFField;
        public UClass* classPrivate;
        public FFieldVariant owner;
        public FField* next;
        public FName namePrivate;
        public EObjectFlags flagsPrivate;
    }
}
