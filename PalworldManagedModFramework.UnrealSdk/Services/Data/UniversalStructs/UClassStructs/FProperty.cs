using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FProperty {
        public FField inheritedFField;

        public int arrayDim;
        public int elementSize;
        public EPropertyFlags propertyFlags;
        public ushort repIndex;
        public ELifetimeCondition blueprintReplicationCondition;
        public int offset_Internal;
        public FName repNotifyFunc;
        public FProperty* propertyLinkNext;
        public FProperty* nextRef;
        public FProperty* destructorLinkNext;
        public FProperty* postConstructLinkNext;
    }
}
