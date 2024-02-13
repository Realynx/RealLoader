using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs {

    /// <summary>
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L361"/>
    /// |
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UStruct/"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0xb0)]
    public unsafe struct UStruct {
        public FNameEntryId ObjectName {
            get {
                return baseUfield.baseUObject.namePrivate.comparisonIndex;
            }
        }
        // Inherits
        public UField baseUfield;

        /*
         * In palworld's case USTRUCT_FAST_ISCHILDOF_IMPL == USTRUCT_ISCHILDOF_STRUCTARRAY = true 
         * So we see this inerhited on it's compile time.
         * https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L363
         */
        public FStructBaseChain inheritedFStructBaseChain;


        // Private 

        /// <summary>
        /// Struct this inherits from, may be null.
        /// </summary>
        public UStruct* superStruct;


        // Public

        /// <summary>
        /// Pointer to start of linked list of child fields.
        /// </summary>
        public UField* children;

        /// <summary>
        /// Pointer to start of linked list of child fields.
        /// </summary>
        public FField* childProperties;

        /// <summary>
        /// Total size of all UProperties, the allocated structure may be larger due to alignment.
        /// </summary>
        public int propertiesSize;

        /// <summary>
        /// Alignment of structure in memory, structure will be at least this large.
        /// </summary>
        public int minAlignment;


        /// <summary>
        /// Script bytecode associated with this object.
        /// </summary>
        public TArray<byte> script;

        /// <summary>
        /// In memory only: Linked list of properties from most-derived to base.
        /// </summary>
        public FProperty* propertyLink;

        /// <summary>
        /// In memory only: Linked list of object reference properties from most-derived to base.
        /// </summary>
        public FProperty* refLink;

        /// <summary>
        /// In memory only: Linked list of properties requiring destruction. Note this does not include things that will be destroyed by the native destructor.
        /// </summary>
        public FProperty* destructorLink;

        /// <summary>
        /// In memory only: Linked list of properties requiring post constructor initialization.
        /// </summary>
        public FProperty* postConstructLink;


        /// <summary>
        /// Array of object references embedded in script code and referenced by FProperties. Mirrored for easy access by realtime garbage collection code
        /// </summary>
        public TArray<nint> scriptAndPropertyObjectReferences;

        // 	typedef TArray<TPair<TFieldPath<FField>, int32>> FUnresolvedScriptPropertiesArray;
        public void* unresolvedScriptProperties;

        /// <summary>
        /// Contains a list of script properties that couldn't be resolved at load time.
        /// </summary>
        public FUnversionedStructSchema* unversionedGameSchema;
    }
}
