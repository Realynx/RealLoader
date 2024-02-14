using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace PalworldManagedModFramework.UnrealSdk.Services {
    public class UnrealReflection {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;

        public UnrealReflection(ILogger logger, IGlobalObjects globalObjects) {
            _logger = logger;
            _globalObjects = globalObjects;
        }

        public unsafe ICollection<FProperty> GetTypeFields(UClass uClass) {
            var fields = new List<FProperty>();
            for (FField* field = uClass.baseUStruct.childProperties; field is not null; field = field->next) {
                fields.Add(*(FProperty*)field);
            }

            return fields;
        }

        public unsafe ICollection<UFunction> GetTypeFunctions(UClass uClass) {
            var fields = new List<UFunction>();
            for (UField* field = uClass.baseUStruct.children; field is not null; field = field->next) {
                fields.Add(*(UFunction*)field);
            }

            return fields;
        }

        public unsafe (FField? returnValue, FField[] parameters) GetFunctionSignature(UFunction uFunction) {
            var hasReturnProperty = uFunction.returnValueOffset != 0xffff;

            var childProps = new List<FField>();
            var x = 0;
            for (FField* currentProp = uFunction.baseUstruct.childProperties; currentProp is not null; currentProp = currentProp->next) {
                childProps.Add(*currentProp);
            }

            var parameters = hasReturnProperty ? childProps[1..] : childProps;
            FField? returnValue = hasReturnProperty ? childProps[0] : null;

            return (returnValue, parameters.ToArray());
        }
    }
}
