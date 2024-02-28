using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace RealLoaderFramework.Sdk.Models.Windows {
    [StructLayout(LayoutKind.Sequential, Size = 0x230)]
    public struct WindowsUClass {
        public UClass uClass;
    }
}
