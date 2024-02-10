using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs {
    [StructLayout(LayoutKind.Explicit)]
    public struct FGCReferenceTokenStream {
        [FieldOffset(0x0)]
        public byte Tokens; // Array

        [FieldOffset(0x10)]
        public int StackSize;

        [FieldOffset(0x14)]
        public EGCTokenType TokenType;
    }
}
