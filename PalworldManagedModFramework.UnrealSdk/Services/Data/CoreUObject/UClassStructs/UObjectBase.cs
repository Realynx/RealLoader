using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.3/en-US/API/Runtime/CoreUObject/UObject/UObjectBase/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectBase.h#L34"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public unsafe struct UObjectBase {
        public void* vptrUObjectBase;

        /// <summary>
        /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectBase.h#L228"/>
        /// </summary>
        public EObjectFlags objectFlags;
        public uint internalIndex;

        /// <summary>
        /// Class the object belongs to. 
        /// </summary>
        public UClass* classPrivate;

        public FName namePrivate;

        /// <summary>
        /// Object this object resides in.
        /// </summary>
        public UObject* outerPrivate;
    }
}
