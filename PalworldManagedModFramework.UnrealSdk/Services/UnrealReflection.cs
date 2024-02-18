using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace PalworldManagedModFramework.UnrealSdk.Services {
    public class UnrealReflection : IUnrealReflection {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;

        public UnrealReflection(ILogger logger, IGlobalObjects globalObjects) {
            _logger = logger;
            _globalObjects = globalObjects;
        }

        public unsafe FProperty*[] GetTypeFields(UClass* uClass) {
            var fields = new List<nint>();
            for (FField* field = uClass->baseUStruct.childProperties; field is not null; field = field->next) {
                fields.Add((nint)field);
            }

            var pFields = new FProperty*[fields.Count];
            for (var x = 0; x < pFields.Length; x++) {
                pFields[x] = (FProperty*)fields[x];
            }

            return pFields;
        }

        public unsafe UFunction*[] GetTypeFunctions(UClass* uClass) {
            var fields = new List<nint>();
            for (UField* field = uClass->baseUStruct.children; field is not null; field = field->next) {
                fields.Add((nint)field);
            }

            var pFields = new UFunction*[fields.Count];
            for (var x = 0; x < pFields.Length; x++) {
                pFields[x] = (UFunction*)fields[x];
            }

            return pFields;
        }

        public unsafe FField*[] GetFunctionSignature(UFunction* uFunction, out FField* returnValue) {
            var hasReturnProperty = uFunction->returnValueOffset != 0xffff;
            var returnValueAddress = uFunction->baseUstruct.childProperties + uFunction->returnValueOffset;

            returnValue = (FField*)IntPtr.Zero;

            var childProps = new List<nint>();
            for (FField* currentProp = uFunction->baseUstruct.childProperties; currentProp is not null; currentProp = currentProp->next) {
                if (hasReturnProperty && currentProp == returnValueAddress) {
                    returnValue = currentProp;
                }
                else {
                    childProps.Add((nint)currentProp);
                }
            }

            var parameters = new FField*[childProps.Count];
            for (var x = 0; x < parameters.Length; x++) {
                parameters[x] = (FField*)childProps[x];
            }

            return parameters;
        }
    }
}
