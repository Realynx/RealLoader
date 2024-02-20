using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.GNameStructs;

namespace PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs {
    /// <summary>
    /// <see href="https://docs.unrealengine.com/5.1/en-US/API/Runtime/CoreUObject/UObject/UClass/"/>
    /// |
    /// <see href="https://github.com/EpicGames/UnrealEngine/blob/5.1/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h#L2573"/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 0x128)]
    public unsafe struct UClass {
        public FNameEntryId ObjectName {
            get {
                return baseUStruct.baseUfield.baseUObject.namePrivate.comparisonIndex;
            }
        }

        // Inherits
        public UStruct baseUStruct;

        // Public
        public void* ClassConstructor;
        public void* ClassVTableHelperCtorCaller;
        public FUObjectCppClassStaticFunctions CppClassStaticFunctions;

        /// <summary>
        /// Class pseudo-unique counter; used to accelerate unique instance name generation.
        /// </summary>
        public int ClassUnique;

        /// <summary>
        /// Index of the first ClassRep that belongs to this class. Anything before that was defined by / belongs to parent classes.
        /// </summary>
        public int FirstOwnedClassRep;

        /// <summary>
        /// Used to check if the class was cooked or not
        /// </summary>
        public bool bCooked;

        /// <summary>
        /// Used to check if the class layout is currently changing and therefore is not ready for a CDO to be created
        /// </summary>
        public bool bLayoutChanging;

        private byte _alignment_a;
        private byte _alignment_b;

        /// <summary>
        /// Class flags; See EClassFlags for more information.
        /// </summary>
        public EClassFlags ClassFlags;

        /// <summary>
        /// Cast flags used to accelerate dynamic_cast&lt;T*> on objects of this type for common T.
        /// </summary>
        public EClassCastFlags ClassCastFlags;

        /// <summary>
        /// The required type for the outer of instances of this class
        /// </summary>
        public UClass* ClassWithin;

        /// <summary>
        /// Which Name.ini file to load Config variables out of.
        /// </summary>
        public FName ClassConfigName;

        /// <summary>
        /// List of replication records.
        /// </summary>
        public TArray<FRepRecord> ClassReps;

        /// <summary>
        /// List of network relevant fields (functions).
        /// </summary>
        public TArray<nint> NetFields;

        /// <summary>
        /// The class default object; used for delta serialization and object initialization.
        /// </summary>
        public UObject* ClassDefaultObject;

        // Protected

        /// <summary>
        /// This is where we store the data that is only changed per class instead of per instance.
        /// </summary>
        public void* SparseClassData;

        /// <summary>
        /// The struct used to store sparse class data.
        /// </summary>
        public UScriptStruct* SparseClassDataStruct;

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
