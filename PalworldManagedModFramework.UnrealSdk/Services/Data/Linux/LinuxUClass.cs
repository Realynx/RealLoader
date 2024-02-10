﻿using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.Linux {

    [StructLayout(LayoutKind.Sequential, Size = 0x270)]
    public struct LinuxUClass {
        public UClass uClass;
    }
}
