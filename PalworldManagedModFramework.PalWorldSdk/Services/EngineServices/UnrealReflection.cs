using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.EngineServices {
    public class UnrealReflection : IUnrealReflection {
        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;

        public UnrealReflection(ILogger logger, INamePoolService namePoolService) {
            _logger = logger;
            _namePoolService = namePoolService;
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

        public unsafe UFunction* GetFunctionAtIndex(UClass* uClass, Index index) {
            var i = 0;
            for (var field = uClass->baseUStruct.children; field is not null; field = field->next) {
                if (i == index.Value) {
                    return (UFunction*)field;
                }

                i++;
            }

            var typeName = _namePoolService.GetNameString(uClass->ObjectName);
            throw new Exception($"Could not find function {index} for {typeName}. Found {i} functions.");
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
