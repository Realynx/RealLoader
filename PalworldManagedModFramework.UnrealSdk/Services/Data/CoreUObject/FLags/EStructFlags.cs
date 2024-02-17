namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags {
    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L722"/>
    /// </summary>
    public enum EStructFlags : int {
        // State flags.
        STRUCT_NoFlags = 0x00000000,
        STRUCT_Native = 0x00000001,

        /// <summary> If set, this struct will be compared using native code </summary>
        STRUCT_IdenticalNative = 0x00000002,

        STRUCT_HasInstancedReference = 0x00000004,

        STRUCT_NoExport = 0x00000008,

        /// <summary> Indicates that this struct should always be serialized as a single unit </summary>
        STRUCT_Atomic = 0x00000010,

        /// <summary> Indicates that this struct uses binary serialization; it is unsafe to add/remove members from this struct without incrementing the package version </summary>
        STRUCT_Immutable = 0x00000020,

        /// <summary> If set, native code needs to be run to find referenced objects </summary>
        STRUCT_AddStructReferencedObjects = 0x00000040,

        /// <summary> Indicates that this struct should be exportable/importable at the DLL layer.  Base structs must also be exportable for this to work. </summary>
        STRUCT_RequiredAPI = 0x00000200,

        /// <summary> If set, this struct will be serialized using the CPP net serializer </summary>
        STRUCT_NetSerializeNative = 0x00000400,

        /// <summary> If set, this struct will be serialized using the CPP serializer </summary>
        STRUCT_SerializeNative = 0x00000800,

        /// <summary> If set, this struct will be copied using the CPP operator= </summary>
        STRUCT_CopyNative = 0x00001000,

        /// <summary> If set, this struct will be copied using memcpy </summary>
        STRUCT_IsPlainOldData = 0x00002000,

        /// <summary> If set, this struct has no destructor and non will be called. STRUCT_IsPlainOldData implies STRUCT_NoDestructor </summary>
        STRUCT_NoDestructor = 0x00004000,

        /// <summary> If set, this struct will not be constructed because it is assumed that memory is zero before construction. </summary>
        STRUCT_ZeroConstructor = 0x00008000,

        /// <summary> If set, native code will be used to export text </summary>
        STRUCT_ExportTextItemNative = 0x00010000,

        /// <summary> If set, native code will be used to export text </summary>
        STRUCT_ImportTextItemNative = 0x00020000,

        /// <summary> If set, this struct will have PostSerialize called on it after CPP serializer or tagged property serialization is complete </summary>
        STRUCT_PostSerializeNative = 0x00040000,

        /// <summary> If set, this struct will have SerializeFromMismatchedTag called on it if a mismatched tag is encountered. </summary>
        STRUCT_SerializeFromMismatchedTag = 0x00080000,

        /// <summary> If set, this struct will be serialized using the CPP net delta serializer </summary>
        STRUCT_NetDeltaSerializeNative = 0x00100000,

        /// <summary> If set, this struct will be have PostScriptConstruct called on it after a temporary object is constructed in a running blueprint </summary>
        STRUCT_PostScriptConstruct = 0x00200000,

        /// <summary> If set, this struct can share net serialization state across connections </summary>
        STRUCT_NetSharedSerialization = 0x00400000,

        /// <summary> If set, this struct has been cleaned and sanitized (trashed) and should not be used </summary>
        STRUCT_Trashed = 0x00800000,

        /// <summary> If set, this structure has been replaced via reinstancing </summary>
        STRUCT_NewerVersionExists = 0x01000000,

        /// <summary> If set, this struct will have CanEditChange on it in the editor to determine if a child property can be edited </summary>
        STRUCT_CanEditChange = 0x02000000,

        /// <summary> Struct flags that are automatically inherited </summary>
        STRUCT_Inherit = STRUCT_HasInstancedReference | STRUCT_Atomic,

        /// <summary> Flags that are always computed, never loaded or done with code generation </summary>
        STRUCT_ComputedFlags = STRUCT_NetDeltaSerializeNative | STRUCT_NetSerializeNative | STRUCT_SerializeNative | STRUCT_PostSerializeNative | STRUCT_CopyNative | STRUCT_IsPlainOldData |
                               STRUCT_NoDestructor | STRUCT_ZeroConstructor | STRUCT_IdenticalNative | STRUCT_AddStructReferencedObjects | STRUCT_ExportTextItemNative | STRUCT_ImportTextItemNative |
                               STRUCT_SerializeFromMismatchedTag | STRUCT_PostScriptConstruct | STRUCT_NetSharedSerialization
    };
}