using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Explicit, Size = 16, Pack = 8)]
    public struct FFieldVariant {
        [FieldOffset(0)]
        public FFieldObjectUnion Container;

        [FieldOffset(8)]
        public bool bIsUObject;
    }
}
