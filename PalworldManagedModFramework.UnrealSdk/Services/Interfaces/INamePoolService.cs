using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services {
    public interface INamePoolService {
        unsafe FNameEntry* GetName(FNameEntryId fnameEntryId);
        string GetNameString(FNameEntryId fnameEntryId);
    }
}