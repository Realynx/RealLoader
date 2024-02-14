using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public unsafe interface INameSpaceGenerator {
        string GetNameSpace(UObjectBaseUtility* baseObject);
    }
}