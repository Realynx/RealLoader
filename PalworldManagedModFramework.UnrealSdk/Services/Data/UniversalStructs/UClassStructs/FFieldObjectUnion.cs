using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Explicit)]
    public struct FFieldObjectUnion {
        [FieldOffset(0)]
        public IntPtr Field; // FField*
        [FieldOffset(0)]
        public IntPtr Object; // UObject*
    }
}
