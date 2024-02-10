using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FUnversionedPropertySerializer {
        public FProperty* property;
        public uint offset;
        public bool bSerializeAsInteger;
        public EIntegerType IntType;
        public byte fastZeroIntNum;
    }
}
