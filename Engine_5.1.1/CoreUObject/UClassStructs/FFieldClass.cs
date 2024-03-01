using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;
using Engine_5._1._1.CoreUObject.GNameStructs;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.3/en-US/API/Runtime/CoreUObject/UObject/FFieldClass/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Field.h#L63"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct FFieldClass {
        public FNameEntryId ObjectName {
            get {
                return name.comparisonIndex;
            }
        }
        /// <summary>
        /// Name of this field class.
        /// </summary>
        public FName name;

        /// <summary>
        /// Unique Id of this field class (for casting).
        /// </summary>
        public ulong id;

        /// <summary>
        /// Cast flags used for casting to other classes.
        /// </summary>
        public ulong castFlags;

        /// <summary>
        /// Class flags.
        /// </summary>
        public EClassFlags classFlags;

        /// <summary>
        /// Super of this class.
        /// </summary>
        public FFieldClass* superClass;

        /// <summary>
        /// Default instance of this class.
        /// </summary>
        public FField* defaultObject;

        /// <summary>
        /// Pointer to a function that can construct an instance of this class.
        /// </summary>
        public FField* constructFn;

        /// <summary>
        /// Counter for generating runtime unique names.
        /// </summary>
        public FThreadSafeCounter uniqueNameIndexCounter;

        // TODO: Implement this soon
        //public unsafe delegate FField* ConstructDefaultObject();
        //public ConstructDefaultObject constructDefaultObject;

        // public
    }
}
