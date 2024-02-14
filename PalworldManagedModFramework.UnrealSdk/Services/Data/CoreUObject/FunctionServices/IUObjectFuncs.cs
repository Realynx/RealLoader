namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices {
    public interface IUObjectFuncs {
        WindowsUObjectFuncs.GetExternalPackageFunc GetExternalPackage { get; set; }
        WindowsUObjectFuncs.GetFullNameFunc GetFullName { get; set; }
        WindowsUObjectFuncs.UObjectBaseUtility_GetOutermost GetParentPackage { get; set; }
    }
}