using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FPackageFileVersion {
        public int fileVersionUE4;
        public int fileVersionUE5;
    }
}
