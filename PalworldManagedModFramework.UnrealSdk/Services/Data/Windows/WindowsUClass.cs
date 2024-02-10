using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.Windows {
    [StructLayout(LayoutKind.Sequential, Size = 0x230)]
    public struct WindowsUClass {
        public UClass uClass;
    }
}
