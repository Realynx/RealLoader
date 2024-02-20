using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Models.CoreUObject;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FString {
        public TArray<byte> data;
    }
}
