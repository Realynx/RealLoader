using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;

namespace Engine_5._1._1.CoreUObject {
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
