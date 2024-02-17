namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags {
    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Script.h#L127"/>
    /// </summary>
    [Flags]
    public enum EFunctionFlags : uint {
        FUNC_None = 0x00000000,

        /// <summary> Function is final (prebindable, non-overridable function). </summary>
        FUNC_Final = 0x00000001,

        /// <summary> Indicates this function is DLL exported/imported. </summary>
        FUNC_RequiredAPI = 0x00000002,

        /// <summary> Function will only run if the object has network authority </summary>
        FUNC_BlueprintAuthorityOnly = 0x00000004,

        /// <summary> Function is cosmetic in nature and should not be invoked on dedicated servers </summary>
        FUNC_BlueprintCosmetic = 0x00000008,

        /// <summary> unused. </summary>
        // FUNC_				= 0x00000010,

        /// <summary> unused. </summary>
        // FUNC_				= 0x00000020,

        /// <summary> Function is network-replicated. </summary>
        FUNC_Net = 0x00000040,

        /// <summary> Function should be sent reliably on the network. </summary>
        FUNC_NetReliable = 0x00000080,

        /// <summary> Function is sent to a net service </summary>
        FUNC_NetRequest = 0x00000100,

        /// <summary> Executable from command line. </summary>
        FUNC_Exec = 0x00000200,

        /// <summary> Native function. </summary>
        FUNC_Native = 0x00000400,

        /// <summary> Event function. </summary>
        FUNC_Event = 0x00000800,

        /// <summary> Function response from a net service </summary>
        FUNC_NetResponse = 0x00001000,

        /// <summary> Static function. </summary>
        FUNC_Static = 0x00002000,

        /// <summary> Function is networked multicast Server -> All Clients </summary>
        FUNC_NetMulticast = 0x00004000,

        /// <summary> Function is used as the merge 'ubergraph' for a blueprint, only assigned when using the persistent 'ubergraph' frame </summary>
        FUNC_UbergraphFunction = 0x00008000,

        /// <summary> Function is a multi-cast delegate signature (also requires FUNC_Delegate to be set!) </summary>
        FUNC_MulticastDelegate = 0x00010000,

        /// <summary> Function is accessible in all classes (if overridden, parameters must remain unchanged). </summary>
        FUNC_Public = 0x00020000,

        /// <summary> Function is accessible only in the class it is defined in (cannot be overridden, but function name may be reused in subclasses.  IOW: if overridden, parameters don't need to match, and Super.Func() cannot be accessed since it's private.) </summary>
        FUNC_Private = 0x00040000,

        /// <summary> Function is accessible only in the class it is defined in and subclasses (if overridden, parameters much remain unchanged). </summary>
        FUNC_Protected = 0x00080000,

        /// <summary> Function is delegate signature (either single-cast or multi-cast, depending on whether FUNC_MulticastDelegate is set.) </summary>
        FUNC_Delegate = 0x00100000,

        /// <summary> Function is executed on servers (set by replication code if passes check) </summary>
        FUNC_NetServer = 0x00200000,

        /// <summary> function has out (pass by reference) parameters </summary>
        FUNC_HasOutParms = 0x00400000,

        /// <summary> function has structs that contain defaults </summary>
        FUNC_HasDefaults = 0x00800000,

        /// <summary> function is executed on clients </summary>
        FUNC_NetClient = 0x01000000,

        /// <summary> function is imported from a DLL </summary>
        FUNC_DLLImport = 0x02000000,

        /// <summary> function can be called from blueprint code </summary>
        FUNC_BlueprintCallable = 0x04000000,

        /// <summary> function can be overridden/implemented from a blueprint </summary>
        FUNC_BlueprintEvent = 0x08000000,

        /// <summary> function can be called from blueprint code, and is also pure (produces no side effects). If you set this, you should set FUNC_BlueprintCallable as well. </summary>
        FUNC_BlueprintPure = 0x10000000,

        /// <summary> function can only be called from an editor script. </summary>
        FUNC_EditorOnly = 0x20000000,

        /// <summary> function can be called from blueprint code, and only reads state (never writes state) </summary>
        FUNC_Const = 0x40000000,

        /// <summary> function must supply a _Validate implementation </summary>
        FUNC_NetValidate = 0x80000000,

        FUNC_AllFlags = 0xFFFFFFFF,
    };
}