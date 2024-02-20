using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Services.Interfaces {
    public interface IGlobalObjects {
        unsafe UObjectBase*[] EnumerateEverything();
        unsafe UObjectBase*[] EnumerateObjects();
        unsafe UObjectBase* FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal);
    }
}