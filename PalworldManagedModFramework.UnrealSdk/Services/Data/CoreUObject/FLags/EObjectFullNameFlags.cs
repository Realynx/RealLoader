namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags {
    /// <summary>
    /// Enum which specifies the mode in which full object names are constructed
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/UObjectBaseUtility.h#L44"/>
    /// </summary>
    public enum EObjectFullNameFlags {
        /// <summary> Standard object full name (i.e. "Type PackageName.ObjectName:SubobjectName") </summary>
        None = 0,

        /// <summary> Adds package to the type portion (i.e. "TypePackage.TypeName PackageName.ObjectName:SubobjectName") </summary>
        IncludeClassPackage = 1,
    };
}