using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FProperty/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UnrealType.h#L220"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FProperty {
        // Inherits
        public FField baseFField;

        public int arrayDim;
        public int elementSize;
        public EPropertyFlags propertyFlags;
        public ushort repIndex;

        // Private
        public ELifetimeCondition blueprintReplicationCondition;
        public int offset_Internal;

        // Public
        public FName repNotifyFunc;
        public FProperty* propertyLinkNext;
        public FProperty* nextRef;
        public FProperty* destructorLinkNext;
        public FProperty* postConstructLinkNext;
    }
}
