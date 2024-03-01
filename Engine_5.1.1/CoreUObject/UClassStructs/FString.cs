using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FString {
        public TArray<byte> data;
    }
}
