using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see cref="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UScriptStruct/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L860"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct UScriptStruct {
        // Inherits
        public UStruct baseUScript;

        // Public
        public EStructFlags structFlags;

        public bool bPrepareCppStructOpsCompleted;

        public ICppStructOps* cppStructOps;
    }
}
