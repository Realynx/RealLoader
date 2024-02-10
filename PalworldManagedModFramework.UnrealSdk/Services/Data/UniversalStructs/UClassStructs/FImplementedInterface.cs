using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FImplementedInterface {
        public UClass* pClass;
        public int pointerOffset;
        public bool bImplementedByK2;
    }
}
