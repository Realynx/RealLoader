using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

namespace RealLoaderFramework.Sdk.Interfaces {
    public interface ICreatableUObject<out TUObject> {
        internal static unsafe abstract TUObject Create(UObject* address, IUnrealReflection unrealReflection, IGlobalObjectsTracker globalObjectsTracker);
    }
}