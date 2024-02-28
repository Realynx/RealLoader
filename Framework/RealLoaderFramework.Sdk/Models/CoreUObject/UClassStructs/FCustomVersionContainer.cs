using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FCustomVersionContainer {
        public TArray<object> versions;
    }
}
