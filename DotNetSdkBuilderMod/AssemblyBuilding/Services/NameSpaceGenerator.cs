using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class NameSpaceGenerator : INameSpaceGenerator {
        private readonly Dictionary<FNameEntryId, string> _namespaces = new();
        private readonly IGlobalObjects _globalObjects;
        private readonly IUObjectFuncs _uObjectFuncs;

        public NameSpaceGenerator(IGlobalObjects globalObjects, IUObjectFuncs uObjectFuncs) {
            _globalObjects = globalObjects;
            _uObjectFuncs = uObjectFuncs;
        }

        public string GetNameSpace(UObjectBase baseObject) {
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(_namespaces, baseObject.namePrivate.comparisonIndex, out var previouslyExisted);

            if (previouslyExisted) {
                return value!;
            }

            var packageName = GetPackageName(baseObject);

            if (packageName == "None") {
                value = "BaseNameSpace";
                return value;
            }

            var nameSpace = new StringBuilder(packageName)
                .Replace('/', '.')
                .Insert(0, "BaseNameSpace")
                .ToString();

            value = nameSpace;
            return value;
        }

        private unsafe string GetPackageName(UObjectBase baseObject) {
            var pBaseObject = &baseObject;
            var package = _uObjectFuncs.GetParentPackage((UObjectBaseUtility*)pBaseObject);

            return _globalObjects.GetNameString(package->Name);
        }
    }
}