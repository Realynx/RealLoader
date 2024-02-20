using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface IPackageNameGenerator {
        string GetPackageName(UObjectBaseUtility* baseObject);
    }
}