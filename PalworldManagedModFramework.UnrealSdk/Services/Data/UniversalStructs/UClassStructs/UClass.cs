using System.Runtime.InteropServices;

using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.GNameStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs {
    [StructLayout(LayoutKind.Sequential, Size = 0x128)]
    public unsafe struct UClass {
        public UStruct baseUStruct;

        public void* ClassConstructor;
        public void* ClassVTableHelperCtorCaller;

        public FUObjectCppClassStaticFunctions CppClassStaticFunctions;

        public int ClassUnique;
        public int FirstOwnedClassRep;

        public bool bCooked;
        public bool bLayoutChanging;

        private byte _alignment_a;
        private byte _alignment_b;

        public EClassFlags ClassFlags;
        public EClassCastFlags ClassCastFlags;
        public UClass* ClassWithin;
        public FName ClassConfigName;

        public TArray<FRepRecord> ClassReps;
        public TArray<nint> NetFields;

        public UObject* ClassDefaultObject;
        public void* SparseClassData;
        public UScriptStruct* SparseClassDataStruct;

        //public TMap<FName, UFunction*, FDefaultSetAllocator, TDefaultMapHashableKeyFuncs<FName, UFunction*,false>> FuncMap;
        //public TMap<FName, UFunction*, FDefaultSetAllocator, TDefaultMapHashableKeyFuncs<FName, UFunction*,false>> SuperFuncMap;

        //public FRWLock SuperFuncMapLock;
        //public TArray<FImplementedInterface> Interfaces;
        //public FGCReferenceTokenStream ReferenceTokenStream;
        //public FCriticalSection ReferenceTokenStreamCritical;
        //public TArray<FNativeFunctionLookup> NativeFunctionLookupTable;
    }
}
