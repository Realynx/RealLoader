using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Interfaces {
    public interface IGlobalObjects {
        IDictionary<string, ICollection<UObjectBase>> EnumerateNamedObjects();
        ICollection<UObjectBase> EnumerateRootObjects();
        ICollection<UObjectBase> EnumerateParents();
        UObjectBase? FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal);
        unsafe FNameEntry* GetName(FNameEntryId fnameEntryId);
        string GetNameString(FNameEntryId fnameEntryId);
        ICollection<UObjectBase> EnumerateEverything();
    }
}