using PalworldManagedModFramework.UnrealSdk.Services.Structs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Interfaces {
    public interface IGlobalObjects {
        IDictionary<string, ICollection<GObjectsStructs.UObjectBase>> EnumerateNamedObjects();
        ICollection<GObjectsStructs.UObjectBase> EnumerateObjects();
        GObjectsStructs.UObjectBase? FindObjects(string name, StringComparison stringComparison = StringComparison.Ordinal);
        unsafe GNamesStructs.FNameEntry* GetName(GNamesStructs.FNameEntryId fnameEntryId);
        string GetNameString(GNamesStructs.FNameEntryId fnameEntryId);
    }
}