using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.GNameStructs {
    /// <summary>
    /// <see cref="https://docs.unrealengine.com/5.1/en-US/API/Runtime/Core/UObject/FNameEntry/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L213"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FNameEntry {
        public FNameEntryHeader header;

        /// <summary>
        /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L222C2-L222C32"/>
        /// Union on ANSICHAR | WIDECHAR, can be either type.
        /// </summary>
        public byte stringContents;
    }
}
