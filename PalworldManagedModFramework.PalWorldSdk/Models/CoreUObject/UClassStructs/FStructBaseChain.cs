using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/FStructBaseChain/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L328"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
    public unsafe struct FStructBaseChain {

        // Private
        public FStructBaseChain** structBaseChainArray;
        public int numStructBasesInChainMinusOne;

        // Alignment was supposed to add the extra 4 bytes, thanks clr :c
        private int _padding;
    }
}
