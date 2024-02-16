using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Interfaces {
    public interface IGlobalObjects {
        unsafe UObjectBase*[] EnumerateEverything();
        unsafe UObjectBase*[] EnumerateObjects();
        unsafe UObjectBase* FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal);
    }
}