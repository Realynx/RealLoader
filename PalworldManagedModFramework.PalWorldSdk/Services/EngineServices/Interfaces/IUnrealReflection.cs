using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IUnrealReflection {
        unsafe FField*[] GetFunctionSignature(UFunction* uFunction, out FField* returnValue, out Index returnValueIndex);
        unsafe FProperty*[] GetTypeFields(UClass* uClass);
        unsafe UFunction*[] GetTypeFunctions(UClass* uClass);
    }
}