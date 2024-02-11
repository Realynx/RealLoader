using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    /// <summary>
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L864"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public class ICppStructOps {
        // Private, https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L1129

        /// <summary>
        /// sizeof() of the structure
        /// </summary>
        [FieldOffset(0x8)]
        public int size;

        /// <summary>
        /// alignof() of the structure.
        /// </summary>
        [FieldOffset(0xc)]
        public int alignment;
    }
}
