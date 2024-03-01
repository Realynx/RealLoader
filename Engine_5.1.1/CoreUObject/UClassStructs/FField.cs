using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;
using Engine_5._1._1.CoreUObject.GNameStructs;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FField/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Field.h#L416"/> 
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x38, Pack = 8)]
    public unsafe struct FField {
        public FNameEntryId ObjectName {
            get {
                return namePrivate.comparisonIndex;
            }
        }

        [FieldOffset(0x0)]
        public void* vptrFField;

        /// <summary>
        /// Pointer to the class object representing the type of this FField.
        /// </summary>
        [FieldOffset(0x8)]
        public FFieldClass* classPrivate;

        /// <summary>
        /// Owner of this field.
        /// </summary>
        [FieldOffset(0x10)]
        public FFieldVariant owner;

        /// <summary>
        /// Next Field in the linked list.
        /// </summary>
        [FieldOffset(0x20)]
        public FField* next;

        /// <summary>
        /// Name of this field.
        /// </summary>
        [FieldOffset(0x28)]
        public FName namePrivate;

        /// <summary>
        /// Object flags.
        /// </summary>
        [FieldOffset(0x30)]
        public EObjectFlags flagsPrivate;
    }
}
