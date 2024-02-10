using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;
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

        public unsafe ICollection<UField> GetTypeFields(UClass uClass) {
            var fields = new List<UField>();
            if (uClass.ClassFlags.HasFlag(EClassFlags.CLASS_None)) {
                return fields;
            }

            for (UField* field = &uClass.baseUStruct.baseUfield; field is not null; field = field->next) {
                fields.Add(*field);
            }

            return fields;
        }

        public unsafe ICollection<FField> GetTypeProperties(UClass uClass) {
            var properties = new List<FField>();
            for (FField* property = uClass.baseUStruct.childProperties; property is not null; property = property->next) {
                properties.Add(*property);
            }

            return properties;
        }
    }
}
