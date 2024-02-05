using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory.Windows {
    public class WindowsClientPattern : IEnginePattern {
        [MachineCodePattern("48 8D 05 ? ? ? ? | EB 13 48 8D 0D ? ? ? ? E8 ? ? ? ? C6 05 ? ? ? ? ? 0F 10", OperandType.RelativeOffset_32)]
        public nint PNamePoolData { get; set; }

        [MachineCodePattern("48 8B 05 ? ? ? ? | 48 8B 0C C8 4C 8D 04 D1 EB 03", OperandType.RelativeOffset_32)]
        public nint PGUObjectArray { get; set; }
    }
}
