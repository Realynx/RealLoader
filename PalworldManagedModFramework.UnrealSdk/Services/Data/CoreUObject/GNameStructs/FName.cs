using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/Core/UObject/FName/"/>
    /// |
    /// <see href=""/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x8)]
    public unsafe struct FName {
        public FNameEntryId comparisonIndex;
        public int number;
    }
}
