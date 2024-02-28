using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace RealLoaderFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IUObjectFuncs {
        public unsafe delegate UPackage* UObjectBaseUtility_GetOutermost(UObjectBaseUtility* instance);

        UObjectBaseUtility_GetOutermost GetParentPackage { get; set; }
    }
}