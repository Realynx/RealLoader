using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace RealLoaderFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IGlobalObjects {
        unsafe UObjectBase*[] EnumerateEverything();
        unsafe UObjectBase*[] EnumerateObjects();
        unsafe UObjectBase* FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal);
    }
}