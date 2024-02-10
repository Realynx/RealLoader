using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
    public unsafe struct FStructBaseChain {
        public FStructBaseChain** structBaseChainArray;
        public int numStructBasesInChainMinusOne;
        private int _padding;
    }
}
