using System.Runtime.InteropServices;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {
    [StructLayout(LayoutKind.Sequential)]
    public struct FCustomVersionContainer {
        public TArray<object> versions;
    }
}
