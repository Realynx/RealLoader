using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class PackageNameGenerator : IPackageNameGenerator {
        private readonly IGlobalObjects _globalObjects;
        private readonly IUObjectFuncs _uObjectFuncs;

        public PackageNameGenerator(IGlobalObjects globalObjects, IUObjectFuncs uObjectFuncs) {
            _globalObjects = globalObjects;
            _uObjectFuncs = uObjectFuncs;
        }

        public unsafe string GetPackageName(UObjectBaseUtility* baseObject) {
            var package = _uObjectFuncs.GetParentPackage(baseObject);
            return _globalObjects.GetNameString(package->Name);
        }
    }
}