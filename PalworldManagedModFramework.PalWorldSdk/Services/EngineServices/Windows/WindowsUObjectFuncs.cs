using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

using static PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces.IUObjectFuncs;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.Windows {
    public unsafe class WindowsUObjectFuncs : IUObjectFuncs {
        public WindowsUObjectFuncs() {
        }

        //[MachineCodePattern("40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 8D AC 24 A0 FE ? ? 48 81 EC 60 02 ? ? 48 8B 05 56 AC ? ? 48 ? ? 48 89 85 50 01 ? ? 48 8D 44 ? ? C6 44 24 ? ?", PatternType.Function)]
        //public GetFullNameFunc GetFullName { get; set; }

        [MachineCodePattern("40 ? 48 83 ? ? 66 66 0F 1F 84 00 00 00 00 ? 48 83 79 ? ? 48 8D ? ? 74 ?  ? ? F7 41 08 00 00 ? ? 74 ? E8 25 50 01 ?", PatternType.Function)]
        public UObjectBaseUtility_GetOutermost GetParentPackage { get; set; }
    }
}
