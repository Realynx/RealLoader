using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject {
    [StructLayout(LayoutKind.Explicit)]
    public class ICppStructOps {
        [FieldOffset(0x8)]
        public int size;

        [FieldOffset(0xc)]
        public int alignment;
    }
}
