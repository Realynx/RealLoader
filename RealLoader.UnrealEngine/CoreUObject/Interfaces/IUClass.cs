using Engine_5._1._1.CoreUObject.Flags;
using Engine_5._1._1.CoreUObject.GNameStructs;

namespace RealLoader.UnrealEngine.CoreUObject.Interfaces {
    public interface IUClass {
        UStruct BaseUStruct { get; set; }
        bool BCooked { get; set; }
        bool BLayoutChanging { get; set; }
        EClassCastFlags ClassCastFlags { get; set; }
        FName ClassConfigName { get; set; }
        unsafe void* ClassConstructor { get; set; }
        unsafe UObject* ClassDefaultObject { get; set; }
        EClassFlags ClassFlags { get; set; }
        TArray<FRepRecord> ClassReps { get; set; }
        int ClassUnique { get; set; }
        unsafe void* ClassVTableHelperCtorCaller { get; set; }
        unsafe UClass* ClassWithin { get; set; }
        FUObjectCppClassStaticFunctions CppClassStaticFunctions { get; set; }
        int FirstOwnedClassRep { get; set; }
        TArray<nint> NetFields { get; set; }
        unsafe void* SparseClassData { get; set; }
        unsafe UScriptStruct* SparseClassDataStruct { get; set; }
    }
}