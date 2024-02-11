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

        public unsafe ICollection<FField> GetTypeFields(UClass uClass) {
            var fields = new List<FField>();
            for (FField* field = uClass.baseUStruct.childProperties; field is not null; field = field->next) {
                fields.Add(*field);
            }

            return fields;
        }
    }
}
