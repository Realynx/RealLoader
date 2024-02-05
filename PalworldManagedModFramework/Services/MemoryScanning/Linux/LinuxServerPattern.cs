using PalworldManagedModFramework.Attributes;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;

namespace PalworldManagedModFramework.Services.MemoryScanning.Linux {
    public class LinuxServerPattern : IEnginePattern {
        [MachineCodePattern("53 89 FB 80 3D ? ? ? ? ? 75 ? BF ? ? ? ? | E8 ? ? ? ? C6 05 ? ? ? ? 01", OperandType.DirectAddress_32)]
        public nint PNamePoolData { get; set; }

        [MachineCodePattern("0F 85 ? ? ? ? BF ? ? ? ? | 4C 89 FE E8 ? ? ? ? E9", OperandType.DirectAddress_32)]
        public nint PGUObjectArray { get; set; }
    }
}
