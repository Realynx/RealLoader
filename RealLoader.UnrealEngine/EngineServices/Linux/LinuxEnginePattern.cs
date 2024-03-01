using RealLoaderFramework.Sdk.Attributes;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

namespace RealLoaderFramework.Sdk.Services.EngineServices.Linux {
    public class LinuxEnginePattern : IEnginePattern {
        [MachineCodePattern("53 89 FB 80 3D ? ? ? ? ? 75 ? BF ? ? ? ? | E8 ? ? ? ? C6 05 ? ? ? ? 01", PatternType.DirectAddress_32)]
        public nint PNamePoolData { get; set; }

        [MachineCodePattern("C6 ? ? ? ? ? ? ? 8B ? ? ? ? ? ? 3B ? ? ? ? ? ? 0F ? ? ? ? ? BF ? ? ? ? | 4C 89 ? E8 ? ? ? ? E9 48", PatternType.DirectAddress_32)]
        public nint PGUObjectArray { get; set; }

        public string ProcessEventPattern { get; } = "55 48 ? ? 41 ? 41 ? 41 ? 41 ? ? 48 81 EC B8 00 ? ? F6 47 ? ? 74 ? 48 8D ? ? ? 41 ? 41 ? 41 ? 41 ? ? C3";

        public string UObject_PostInitPropertiesPattern { get; } = "48 ? ? 48 8B ? ? 48 ? ? 48 8B 80 A0 03 ? ? BA 01 00 ? ? ? ? 45 ? ? FF ?";

        public string UObject_BeginDestroyPattern { get; } = "53 48 83 ? ? 48 ? ? 66 83 7F ? ? 78 ? 48 8B ? ? 48 89 44 ? ? 48 8D 7C ? ? 48 8D 74 ? ?";

        public string UObject_FinishDestroyPattern { get; } = "53 48 83 ? ? 48 ? ? F6 47 ? ? 75 ? 48 8B ? ? 48 89 44 ? ? 48 8D 7C ? ?";
    }
}
