using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Services.Interfaces {
    public interface IUnrealReflection {
        unsafe FField*[] GetFunctionSignature(UFunction* uFunction, out FField* returnValue);
        unsafe FProperty*[] GetTypeFields(UClass* uClass);
        unsafe UFunction*[] GetTypeFunctions(UClass* uClass);
    }
}