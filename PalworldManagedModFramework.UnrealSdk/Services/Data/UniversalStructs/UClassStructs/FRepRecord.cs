using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FRepRecord {
        public FProperty* property;
        public int index;
    }
}
