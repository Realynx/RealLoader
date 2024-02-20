using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FGuid {
        public uint a;
        public uint b;
        public uint c;
        public uint d;
    }
}
