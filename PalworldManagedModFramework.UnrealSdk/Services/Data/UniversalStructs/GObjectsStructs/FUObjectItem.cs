using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GobjectsStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FUObjectItem {
        public UObjectBase* uObject;
        public uint flags;
        public int clusterRootIndex;
        public int serialNumber;
        public int _padding;
    }
}
