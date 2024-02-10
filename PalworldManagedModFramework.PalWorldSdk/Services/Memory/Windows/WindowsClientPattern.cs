using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Memory.Windows {
    public class WindowsClientPattern : IEnginePattern {
        [MachineCodePattern("48 8D 05 ? ? ? ? | EB 13 48 8D 0D ? ? ? ? E8 ? ? ? ? C6 05 ? ? ? ? ? 0F 10", OperandType.IP_RelativeOffset_32)]
        public nint PNamePoolData { get; set; }

        [MachineCodePattern("48 89 ? ? ? ? ? 48 89 ? ? ? ? ? 89 ? ? ? ? ? 89 ? ? ? ? ? | 85 ? 7f ? 48 8d ? ? ? ? ? 48 8d ? ? ? ? ? ? e8", OperandType.IP_RelativeOffset_32)]
        public nint PGUObjectArray { get; set; }
    }
}