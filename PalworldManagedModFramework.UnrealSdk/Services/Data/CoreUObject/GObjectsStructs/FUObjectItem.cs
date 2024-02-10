using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GobjectsStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FUObjectItem {
        public UObjectBase* uObject;
        public uint flags;
        public int clusterRootIndex;
        public int serialNumber;
        public int _padding;
    }
}
