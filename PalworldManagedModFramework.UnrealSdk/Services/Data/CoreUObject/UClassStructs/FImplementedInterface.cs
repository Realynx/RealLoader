using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FImplementedInterface/"/>
    /// |
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L2506"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct FImplementedInterface {
        /// <summary>
        ///  the interface class.
        /// </summary>
        public UClass* pClass;

        /// <summary>
        /// the pointer offset of the interface's vtable.
        /// </summary>
        public int pointerOffset;

        /// <summary>
        /// whether or not this interface has been implemented via K2.
        /// </summary>
        public bool bImplementedByK2;
    }
}
