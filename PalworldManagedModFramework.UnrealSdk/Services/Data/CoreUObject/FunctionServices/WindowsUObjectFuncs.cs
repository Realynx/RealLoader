using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices {
    public unsafe class WindowsUObjectFuncs : IUObjectFuncs {
        public WindowsUObjectFuncs() {

        }

        [MachineCodePattern("48 83 79 ? ? 75 ? 48 ? ? C3 ? ? F7 41 08 00 00 ? ? 0F 85 46 5A 01 ?", OperandType.Function)]
        public GetExternalPackageFunc GetExternalPackage { get; set; }
        public delegate UPackage* GetExternalPackageFunc(UObjectBase* instance);

        [MachineCodePattern("40 ? ? ? 41 ? 41 ? 41 ? 41 ? 48 8D AC 24 A0 FE ? ? 48 81 EC 60 02 ? ? 48 8B 05 56 AC ? ? 48 ? ? 48 89 85 50 01 ? ? 48 8D 44 ? ? C6 44 24 ? ?", OperandType.Function)]
        public GetFullNameFunc GetFullName { get; set; }
        public delegate void GetFullNameFunc(UObjectBaseUtility* instance, UObject* param1, FString* param2, EObjectFullNameFlags param3);

        [MachineCodePattern("40 ? 48 83 ? ? 66 66 0F 1F 84 00 00 00 00 ? 48 83 79 ? ? 48 8D ? ? 74 ?  ? ? F7 41 08 00 00 ? ? 74 ? E8 25 50 01 ?", OperandType.Function)]
        public UObjectBaseUtility_GetOutermost GetParentPackage { get; set; }
        public delegate UPackage* UObjectBaseUtility_GetOutermost(UObjectBaseUtility* instance);
    }
}
