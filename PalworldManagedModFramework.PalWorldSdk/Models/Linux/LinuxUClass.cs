using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Models.Linux {

    [StructLayout(LayoutKind.Sequential, Size = 0x270)]
    public struct LinuxUClass {
        public UClass uClass;
    }
}
