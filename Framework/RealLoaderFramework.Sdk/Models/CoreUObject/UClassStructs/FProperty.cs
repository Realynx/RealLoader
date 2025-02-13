using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;
using RealLoaderFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FProperty/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UnrealType.h#L220"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FProperty {
        public FNameEntryId ObjectName {
            get {
                return baseFField.namePrivate.comparisonIndex;
            }
        }
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
