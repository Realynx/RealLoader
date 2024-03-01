using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;
using Engine_5._1._1.CoreUObject.GNameStructs;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FProperty/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UnrealType.h#L220"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x78)]
    public unsafe struct FProperty {
        public FNameEntryId ObjectName {
            get {
                return baseFField.namePrivate.comparisonIndex;
            }
        }
        // Inherits
        [FieldOffset(0x00)]
        public FField baseFField;

        [FieldOffset(0x38)]
        public int arrayDim;

        [FieldOffset(0x3C)]
        public int elementSize;

        [FieldOffset(0x40)]
        public EPropertyFlags propertyFlags;

        [FieldOffset(0x48)]
        public ushort repIndex;

        // Private
        [FieldOffset(0x4a)]
        public ELifetimeCondition blueprintReplicationCondition;

        [FieldOffset(0x4c)]
        public int offset_Internal;

        // Public
        [FieldOffset(0x50)]
        public FName repNotifyFunc;

        [FieldOffset(0x58)]
        public FProperty* propertyLinkNext;

        [FieldOffset(0x60)]
        public FProperty* nextRef;

        [FieldOffset(0x68)]
        public FProperty* destructorLinkNext;

        [FieldOffset(0x70)]
        public FProperty* postConstructLinkNext;
    }
}
