using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FPackagePath {
        public FName packageName;
        public EPackageExtension headerExtension;
    }
}
