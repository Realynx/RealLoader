using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FUnversionedStructSchema {
        public uint num;
        public FUnversionedPropertySerializer* Serializers;
    }
}
