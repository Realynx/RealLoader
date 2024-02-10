using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FName {
        public FNameEntryId comparisonIndex;
        public int number;
    }
}
