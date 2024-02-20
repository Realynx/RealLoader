using PalworldManagedModFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.Sdk.Services.Interfaces {
    public interface INamePoolService {
        unsafe FNameEntry* GetName(FNameEntryId fnameEntryId);
        string GetNameString(FNameEntryId fnameEntryId);
    }
}