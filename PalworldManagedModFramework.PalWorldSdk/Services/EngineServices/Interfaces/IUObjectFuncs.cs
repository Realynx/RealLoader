using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IUObjectFuncs {
        public unsafe delegate UPackage* UObjectBaseUtility_GetOutermost(UObjectBaseUtility* instance);

        UObjectBaseUtility_GetOutermost GetParentPackage { get; set; }
    }
}