using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;

namespace PalworldManagedModFramework.Services.MemoryScanning.Linux {
    public class LinuxServerPattern : IEnginePattern {
        [MachineCodePattern("53 89 FB 80 3D ? ? ? ? ? 75 ? BF ? ? ? ? | E8 ? ? ? ? C6 05 ? ? ? ? 01", PatternType.DirectAddress_32)]
        public nint PNamePoolData { get; set; }

        [MachineCodePattern("C6 ? ? ? ? ? ? ? 8B ? ? ? ? ? ? 3B ? ? ? ? ? ? 0F ? ? ? ? ? BF ? ? ? ? | 4C 89 ? E8 ? ? ? ? E9 48", PatternType.DirectAddress_32)]
        public nint PGUObjectArray { get; set; }
    }
}
