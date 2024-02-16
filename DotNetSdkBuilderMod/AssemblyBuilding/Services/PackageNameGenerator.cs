using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class PackageNameGenerator : IPackageNameGenerator {
        private readonly INamePoolService _namePoolService;
        private readonly IUObjectFuncs _uObjectFuncs;

        public PackageNameGenerator(INamePoolService namePoolService, IUObjectFuncs uObjectFuncs) {
            _namePoolService = namePoolService;
            _uObjectFuncs = uObjectFuncs;
        }

        public unsafe string GetPackageName(UObjectBaseUtility* baseObject) {
            var package = _uObjectFuncs.GetParentPackage(baseObject);
            return _namePoolService.GetNameString(package->Name);
        }
    }
}