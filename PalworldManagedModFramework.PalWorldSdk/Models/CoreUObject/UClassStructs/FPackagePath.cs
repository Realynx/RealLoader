using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FPackagePath {
        public FName packageName;
        public EPackageExtension headerExtension;
    }
}
