using DotNetSdkBuilderMod.AssemblyBuilding.Models;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface IPackageNameGenerator {
        string GetPackageName(UObjectBaseUtility* baseObject);
    }
}