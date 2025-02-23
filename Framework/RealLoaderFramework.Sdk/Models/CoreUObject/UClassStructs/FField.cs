﻿using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;
using RealLoaderFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FField/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Field.h#L416"/> 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x38, Pack = 8)]
    public unsafe struct FField {
        public FNameEntryId ObjectName {
            get {
                return namePrivate.comparisonIndex;
            }
        }
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
