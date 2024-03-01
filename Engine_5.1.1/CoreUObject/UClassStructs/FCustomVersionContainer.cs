using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct FCustomVersionContainer {
        [FieldOffset(0x0)]
        public TArray<object> versions;
    }
}
