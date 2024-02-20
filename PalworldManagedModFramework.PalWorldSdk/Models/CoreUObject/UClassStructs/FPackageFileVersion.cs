using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FPackageFileVersion {
        public int fileVersionUE4;
        public int fileVersionUE5;
    }
}
