using RealLoaderFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace RealLoaderFramework.Sdk.Services.EngineServices.Interfaces {
    public interface INamePoolService {
        unsafe FNameEntry* GetName(FNameEntryId fnameEntryId);
        string GetNameString(FNameEntryId fnameEntryId);
        string GetSanitizedNameString(FNameEntryId fnameEntryId);
    }
}