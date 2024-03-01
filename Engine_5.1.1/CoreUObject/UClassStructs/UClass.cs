using System.Runtime.InteropServices;

using Engine_5._1._1.CoreUObject.Flags;
using Engine_5._1._1.CoreUObject.GNameStructs;

namespace Engine_5._1._1.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UClass/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L2573"/>
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 0x128)]
    public unsafe struct UClass : IUClass<UStruct> {
        // Inherits
        [FieldOffset(0x00)]
        private UStruct baseUStruct;

        // Public
        [FieldOffset(0xb0)]
        private void* classConstructor;

        [FieldOffset(0xb8)]
        private void* classVTableHelperCtorCaller;

        [FieldOffset(0xc8)]
        private FUObjectCppClassStaticFunctions cppClassStaticFunctions;

        /// <summary>
        /// Class pseudo-unique counter; used to accelerate unique instance name generation.
        /// </summary>
        [FieldOffset(0xc8)]
        private int classUnique;

        /// <summary>
        /// Index of the first ClassRep that belongs to this class. Anything before that was defined by / belongs to parent classes.
        /// </summary>
        [FieldOffset(0xcc)]
        private int firstOwnedClassRep;

        /// <summary>
        /// Used to check if the class was cooked or not
        /// </summary>
        [FieldOffset(0xD0)]
        private bool bCooked;

        /// <summary>
        /// Used to check if the class layout is currently changing and therefore is not ready for a CDO to be created
        /// </summary>
        [FieldOffset(0xd1)]
        private bool bLayoutChanging;

        /// <summary>
        /// Class flags; See EClassFlags for more information.
        /// </summary>
        [FieldOffset(0xd4)]
        private EClassFlags classFlags;

        /// <summary>
        /// Cast flags used to accelerate dynamic_cast&lt;T*> on objects of this type for common T.
        /// </summary>
        [FieldOffset(0xd8)]
        private EClassCastFlags classCastFlags;

        /// <summary>
        /// The required type for the outer of instances of this class
        /// </summary>
        [FieldOffset(0xe0)]
        private UClass* classWithin;

        /// <summary>
        /// Which Name.ini file to load Config variables out of.
        /// </summary>
        [FieldOffset(0xe8)]
        private FName classConfigName;

        /// <summary>
        /// List of replication records.
        /// </summary>
        [FieldOffset(0xF0)]
        private TArray<FRepRecord> classReps; // 0x10 bytes

        /// <summary>
        /// List of network relevant fields (functions).
        /// </summary>
        [FieldOffset(0x100)]
        private TArray<nint> netFields; // 0x10 bytes

        /// <summary>
        /// The class default object; used for delta serialization and object initialization.
        /// </summary>
        [FieldOffset(0x110)]
        private UObject* classDefaultObject;

        // Protected

        /// <summary>
        /// This is where we store the data that is only changed per class instead of per instance.
        /// </summary>
        [FieldOffset(0x118)]
        private void* sparseClassData;

        /// <summary>
        /// The struct used to store sparse class data.
        /// </summary>
        [FieldOffset(0x120)]
        private UScriptStruct* sparseClassDataStruct;

        /// <summary>
        ///  Map of all functions by name contained in this class.
        /// </summary>
        // public TMap<FName, UFunction*, FDefaultSetAllocator, TDefaultMapHashableKeyFuncs<FName, UFunction*,false>> FuncMap;

        /// <summary>
        ///  A cache of all functions by name that exist in a parent (superclass or interface) context.
        /// </summary>
        //public TMap<FName, UFunction*, FDefaultSetAllocator, TDefaultMapHashableKeyFuncs<FName, UFunction*,false>> SuperFuncMap;

        /// <summary>
        ///  Scope lock to avoid the SuperFuncMap being read and written to simultaneously on multiple threads.
        /// </summary>
        //public FRWLock SuperFuncMapLock;

        //public TArray<FImplementedInterface> Interfaces;
        //public FGCReferenceTokenStream ReferenceTokenStream;
        //public FCriticalSection ReferenceTokenStreamCritical;
        //public TArray<FNativeFunctionLookup> NativeFunctionLookupTable;
    }
}
