using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;

namespace RealLoaderFramework.Sdk.Models.CoreUObject {
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
