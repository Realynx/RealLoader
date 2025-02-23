﻿using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;

namespace RealLoaderFramework.Sdk.Services.EngineServices.Interfaces {
    public interface IUnrealReflection {
        unsafe FField*[] GetFunctionSignature(UFunction* uFunction, out FField* returnValue, out Index returnValueIndex);
        unsafe FProperty*[] GetTypeFields(UClass* uClass);
        unsafe UFunction*[] GetTypeFunctions(UClass* uClass);
        unsafe UFunction*[] GetTypeFunctions(UStruct* uStruct);
        unsafe UFunction* GetFunctionAtIndex(UStruct* uStruct, Index index);
    }
}