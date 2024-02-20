using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Models.Windows {
    [StructLayout(LayoutKind.Sequential, Size = 0x230)]
    public struct WindowsUClass {
        public UClass uClass;
    }
}
