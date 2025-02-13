using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject;

namespace RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FString {
        public TArray<byte> data;
    }
}
