using System.Runtime.InteropServices;

using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace RealLoaderFramework.Sdk.Models.Linux {

    [StructLayout(LayoutKind.Sequential, Size = 0x270)]
    public struct LinuxUClass {
        public UClass uClass;
    }
}
