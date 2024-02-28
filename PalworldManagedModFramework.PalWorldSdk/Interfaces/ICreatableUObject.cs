using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

namespace PalworldManagedModFramework.Sdk.Interfaces {
    public interface ICreatableUObject<out TUObject> {
        internal static unsafe abstract TUObject Create(UObject* address, IUnrealReflection unrealReflection, IGlobalObjectsTracker globalObjectsTracker);
    }
}