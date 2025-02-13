using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;
using RealLoaderFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FPackagePath {
        public FName packageName;
        public EPackageExtension headerExtension;
    }
}
