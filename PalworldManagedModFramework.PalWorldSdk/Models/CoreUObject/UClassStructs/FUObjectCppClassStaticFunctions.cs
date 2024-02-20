using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/ObjectMacros.h#L1902"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FUObjectCppClassStaticFunctions {
        /// <summary>
        /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/ObjectMacros.h#L1992"/>
        /// </summary>
        public void* addReferencedObjects;
    }
}
