using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services;

public class NameSpaceGenerator {
    private unsafe string GetPackageName(ClassNode rootNode) {
        var baseObject = rootNode.nodeClass.baseUStruct.baseUfield.baseUObject;
        var pBaseObject = &baseObject;
        var package = _uObjectFuncs.GetParentPackage((UObjectBaseUtility*)pBaseObject);

        return _globalObjects.GetNameString(package->Name);
    }
}