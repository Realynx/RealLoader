using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services {
    public class UnrealReflection : IUnrealReflection {
        private readonly ILogger _logger;

        public UnrealReflection(ILogger logger) {
            _logger = logger;
        }

        public unsafe FProperty*[] GetTypeFields(UClass* uClass) {
            var fields = new List<nint>();
            for (var field = uClass->baseUStruct.childProperties; field is not null; field = field->next) {
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
            for (var field = uClass->baseUStruct.children; field is not null; field = field->next) {
                fields.Add((nint)field);
            }

            var pFields = new UFunction*[fields.Count];
            for (var x = 0; x < pFields.Length; x++) {
                pFields[x] = (UFunction*)fields[x];
            }

            return pFields;
        }

        public unsafe FField*[] GetFunctionSignature(UFunction* uFunction, out FField* returnValue, out Index returnValueIndex) {
            var hasReturnProperty = uFunction->returnValueOffset != 0xffff;
            var returnValueAddress = uFunction->baseUstruct.childProperties + uFunction->returnValueOffset;

            returnValue = (FField*)nint.Zero;
            returnValueIndex = 0;

            var i = 0;
            var childProps = new List<nint>();
            for (var currentProp = uFunction->baseUstruct.childProperties; currentProp is not null; currentProp = currentProp->next) {
                if (hasReturnProperty && currentProp == returnValueAddress) {
                    returnValue = currentProp;
                    returnValueIndex = i;
                }
                else {
                    childProps.Add((nint)currentProp);
                }

                i++;
            }

            var parameters = new FField*[childProps.Count];
            for (var x = 0; x < parameters.Length; x++) {
                parameters[x] = (FField*)childProps[x];
            }

            return parameters;
        }
    }
}
