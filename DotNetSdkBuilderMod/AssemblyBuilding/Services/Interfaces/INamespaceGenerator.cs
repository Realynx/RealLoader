using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces {
    public interface INameSpaceGenerator {
        string GetNameSpace(UObjectBase baseObject);
    }
}