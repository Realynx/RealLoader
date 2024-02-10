using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FField/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Field.h#L416"/> 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct FField {
        public void* vptrFField;

        /// <summary>
        /// Pointer to the class object representing the type of this FField.
        /// </summary>
        public FFieldClass* classPrivate;

        /// <summary>
        /// Owner of this field.
        /// </summary>
        public FFieldVariant owner;

        /// <summary>
        /// Next Field in the linked list.
        /// </summary>
        public FField* next;

        /// <summary>
        /// Name of this field.
        /// </summary>
        public FName namePrivate;

        /// <summary>
        /// Object flags.
        /// </summary>
        public EObjectFlags flagsPrivate;
    }
}
