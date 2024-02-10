using PalworldManagedModFramework.PalWorldSdk.Attributes;
using PalworldManagedModFramework.PalWorldSdk.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.UniversalStructs.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger, IGlobalObjects globalObjects) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _globalObjects = globalObjects;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");
            DumpAllMembers();
        }

        private unsafe void DumpAllMembers() {
            var parents = _globalObjects.EnumerateEverything();

            foreach (var parentObject in parents) {
                var objectName = _globalObjects.GetNameString(parentObject.namePrivate.comparisonIndex);
                _logger.Debug($"[{objectName}] Dumping All Members");

                DumpClassFields(*parentObject.classPrivate);
                DumpClassProperties(*parentObject.classPrivate);
            }
        }

        private unsafe void DumpClassFields(UClass uClass) {
            _logger.Debug("Dump Fields");

            for (UField* nextField = &uClass.baseUStruct.baseUfield; nextField is not null; nextField = nextField->next) {
                var fieldNameID = nextField->baseUObject.namePrivate;
                var fieldName = _globalObjects.GetNameString(fieldNameID.comparisonIndex);

                _logger.Debug($"Field Name: {fieldName}");
            }
        }

        private unsafe void DumpClassProperties(UClass uClass) {
            _logger.Debug("Dump Properties");

            for (FField* nextField = uClass.baseUStruct.childProperties; nextField is not null; nextField = nextField->next) {
                var fieldNameID = nextField->namePrivate;
                var propertyName = _globalObjects.GetNameString(fieldNameID.comparisonIndex);

                _logger.Debug($"Property Name: {propertyName}");
            }
        }

        public void Unload() {

        }
    }
}
