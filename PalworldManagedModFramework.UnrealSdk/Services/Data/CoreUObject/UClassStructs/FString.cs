using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FString {
        public TArray<byte> data;
    }
}
