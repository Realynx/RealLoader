using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.Windows {
    public class WindowsEnginePattern : IEnginePattern {
        [MachineCodePattern("48 8D 05 ? ? ? ? | EB 13 48 8D 0D ? ? ? ? E8 ? ? ? ? C6 05 ? ? ? ? ? 0F 10", PatternType.IP_RelativeOffset_32)]
        public nint PNamePoolData { get; set; }

        [MachineCodePattern("48 89 ? ? ? ? ? 48 89 ? ? ? ? ? 89 ? ? ? ? ? 89 ? ? ? ? ? | 85 ? 7f ? 48 8d ? ? ? ? ? 48 8d ? ? ? ? ? ? e8", PatternType.IP_RelativeOffset_32)]
        public nint PGUObjectArray { get; set; }

        // https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Private/UObject/ScriptCore.cpp#L1963
        //  // 40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 81 EC 10 01 ? ? 48 8D 6C ? ? 48 89 9D 38 01
        // https://github.com/EpicGames/UnrealEngine/blob/4.27/Engine/Source/Runtime/CoreUObject/Private/UObject/ScriptCore.cpp#L1848
        //  // 48 89 ? ? 48 ? ? 48 8B ? ? 48 ? ? 48 89 ? ? 4C 89 ? ? 4C 8D 3D 1A 37
        public string ProcessEventPattern { get; } = "40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 81 EC 10 01 ? ? 48 8D 6C ? ? 48 89 9D 38 01";

        public string UObject_PostInitPropertiesPattern { get; } = "48 83 ? ? 48 ? ? 48 C7 44 24 20 00 00 ? ? 48 8B ? ? 45 ? ? 41 ? ?";

        public string UObject_BeginDestroyPattern { get; } = "40 ? 48 83 ? ? 8B ? ? 48 ? ? C1 ? ? ? ? 75 ? 48 8B ? ? 48 8D 54";

        public string UObject_FinishDestroyPattern { get; } = "40 ? 48 83 ? ? F6 41 ? ? 48 ? ? 75 ? 48 8B ? ? 48 8D 54 ? ? 48 8D 4C";

    }
}