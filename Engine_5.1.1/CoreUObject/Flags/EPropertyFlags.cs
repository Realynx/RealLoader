namespace Engine_5._1._1.CoreUObject.Flags {
    /// <summary>
    /// <see cref="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/ObjectMacros.h#L393"/>
    /// </summary>
    [Flags]
    public enum EPropertyFlags : ulong {
        CPF_None = 0,

        /// <summary> Property is user-settable in the editor. </summary>
        CPF_Edit = 0x0000000000000001,

        /// <summary> This is a constant function parameter </summary>
        CPF_ConstParm = 0x0000000000000002,

        /// <summary> This property can be read by blueprint code </summary>
        CPF_BlueprintVisible = 0x0000000000000004,

        /// <summary> Object can be exported with actor. </summary>
        CPF_ExportObject = 0x0000000000000008,

        /// <summary> This property cannot be modified by blueprint code </summary>
        CPF_BlueprintReadOnly = 0x0000000000000010,

        /// <summary> Property is relevant to network replication. </summary>
        CPF_Net = 0x0000000000000020,

        /// <summary> Indicates that elements of an array can be modified, but its size cannot be changed. </summary>
        CPF_EditFixedSize = 0x0000000000000040,

        /// <summary> Function/When call parameter. </summary>
        CPF_Parm = 0x0000000000000080,

        /// <summary> Value is copied out after function call. </summary>
        CPF_OutParm = 0x0000000000000100,

        /// <summary> memset is fine for construction </summary>
        CPF_ZeroConstructor = 0x0000000000000200,

        /// <summary> Return value. </summary>
        CPF_ReturnParm = 0x0000000000000400,

        /// <summary> Disable editing of this property on an archetype/sub-blueprint </summary>
        CPF_DisableEditOnTemplate = 0x0000000000000800,

        /// <summary> Object property can never be null </summary>
        CPF_NonNullable = 0x0000000000001000,

        /// <summary> Property is transient: shouldn't be saved or loaded, except for Blueprint CDOs. </summary>
        CPF_Transient = 0x0000000000002000,

        /// <summary> Property should be loaded/saved as permanent profile. </summary>
        CPF_Config = 0x0000000000004000,

        /// <summary> Parameter must be linked explicitly in blueprint. Leaving the parameter out results in a compile error. </summary>
        CPF_RequiredParm = 0x0000000000008000,

        /// <summary> Disable editing on an instance of this class </summary>
        CPF_DisableEditOnInstance = 0x0000000000010000,

        /// <summary> Property is uneditable in the editor. </summary>
        CPF_EditConst = 0x0000000000020000,

        /// <summary> Load config from base class, not subclass. </summary>
        CPF_GlobalConfig = 0x0000000000040000,

        /// <summary> Property is a component references. </summary>
        CPF_InstancedReference = 0x0000000000080000,

        //CPF_        = 0x0000000000100000,

        /// <summary> Property should always be reset to the default value during any type of duplication (copy/paste, binary duplication, etc.) </summary>
        CPF_DuplicateTransient = 0x0000000000200000,

        //CPF_        = 0x0000000000400000,
        //CPF_           = 0x0000000000800000,

        /// <summary> Property should be serialized for save games, this is only checked for game-specific archives with ArIsSaveGame </summary>
        CPF_SaveGame = 0x0000000001000000,

        /// <summary> Hide clear button. </summary>
        CPF_NoClear = 0x0000000002000000,

        /// <summary> </summary>
        //CPF_         = 0x0000000004000000,
        /// <summary> Value is passed by reference; CPF_OutParam and CPF_Param should also be set. </summary>
        CPF_ReferenceParm = 0x0000000008000000,

        /// <summary> MC Delegates only.  Property should be exposed for assigning in blueprint code </summary>
        CPF_BlueprintAssignable = 0x0000000010000000,

        /// <summary> Property is deprecated.  Read it from an archive, but don't save it. </summary>
        CPF_Deprecated = 0x0000000020000000,

        /// <summary> If this is set, then the property can be memcopied instead of CopyCompleteValue / CopySingleValue </summary>
        CPF_IsPlainOldData = 0x0000000040000000,

        /// <summary> Not replicated. For non replicated properties in replicated structs </summary>
        CPF_RepSkip = 0x0000000080000000,

        /// <summary> Notify actors when a property is replicated </summary>
        CPF_RepNotify = 0x0000000100000000,

        /// <summary> interpolatable property for use with cinematics </summary>
        CPF_Interp = 0x0000000200000000,

        /// <summary> Property isn't transacted </summary>
        CPF_NonTransactional = 0x0000000400000000,

        /// <summary> Property should only be loaded in the editor </summary>
        CPF_EditorOnly = 0x0000000800000000,

        /// <summary> No destructor </summary>
        CPF_NoDestructor = 0x0000001000000000,

        //CPF_        = 0x0000002000000000,

        /// <summary> Only used for weak pointers, means the export type is autoweak </summary>
        CPF_AutoWeak = 0x0000004000000000,

        /// <summary> Property contains component references. </summary>
        CPF_ContainsInstancedReference = 0x0000008000000000,

        /// <summary> asset instances will add properties with this flag to the asset registry automatically </summary>
        CPF_AssetRegistrySearchable = 0x0000010000000000,

        /// <summary> The property is visible by default in the editor details view </summary>
        CPF_SimpleDisplay = 0x0000020000000000,

        /// <summary> The property is advanced and not visible by default in the editor details view </summary>
        CPF_AdvancedDisplay = 0x0000040000000000,

        /// <summary> property is protected from the perspective of script </summary>
        CPF_Protected = 0x0000080000000000,

        /// <summary> MC Delegates only.  Property should be exposed for calling in blueprint code </summary>
        CPF_BlueprintCallable = 0x0000100000000000,

        /// <summary> MC Delegates only.  This delegate accepts (only in blueprint) only events with BlueprintAuthorityOnly. </summary>
        CPF_BlueprintAuthorityOnly = 0x0000200000000000,

        /// <summary> Property shouldn't be exported to text format (e.g. copy/paste) </summary>
        CPF_TextExportTransient = 0x0000400000000000,

        /// <summary> Property should only be copied in PIE </summary>
        CPF_NonPIEDuplicateTransient = 0x0000800000000000,

        /// <summary> Property is exposed on spawn </summary>
        CPF_ExposeOnSpawn = 0x0001000000000000,

        /// <summary> A object referenced by the property is duplicated like a component. (Each actor should have an own instance.) </summary>
        CPF_PersistentInstance = 0x0002000000000000,

        /// <summary> Property was parsed as a wrapper class like TSubclassOf&lt;T>, FScriptInterface etc., rather than a USomething* </summary>
        CPF_UObjectWrapper = 0x0004000000000000,

        /// <summary> This property can generate a meaningful hash value. </summary>
        CPF_HasGetValueTypeHash = 0x0008000000000000,

        /// <summary> Public native access specifier </summary>
        CPF_NativeAccessSpecifierPublic = 0x0010000000000000,

        /// <summary> Protected native access specifier </summary>
        CPF_NativeAccessSpecifierProtected = 0x0020000000000000,

        /// <summary> Private native access specifier </summary>
        CPF_NativeAccessSpecifierPrivate = 0x0040000000000000,

        /// <summary> Property shouldn't be serialized, can still be exported to text </summary>
        CPF_SkipSerialization = 0x0080000000000000,
    }
}