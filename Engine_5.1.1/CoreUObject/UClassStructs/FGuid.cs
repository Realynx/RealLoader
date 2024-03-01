using System.Runtime.InteropServices;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FGuid {
        public uint a;
        public uint b;
        public uint c;
        public uint d;
    }
}
