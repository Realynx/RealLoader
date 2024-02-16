using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.UnrealSdk.Services.Interfaces {
    public interface IUnrealReflection {
        unsafe FField*[] GetFunctionSignature(UFunction* uFunction, out FField* returnValue);
        unsafe FProperty*[] GetTypeFields(UClass* uClass);
        unsafe UFunction*[] GetTypeFunctions(UClass* uClass);
    }
}