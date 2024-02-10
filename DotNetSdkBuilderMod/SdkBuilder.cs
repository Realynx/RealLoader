using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly UnrealReflection _unrealReflection;
        private readonly SourceCodeGenerator _sourceCodeGenerator;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger,
            IGlobalObjects globalObjects, UnrealReflection unrealReflection, SourceCodeGenerator sourceCodeGenerator) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _globalObjects = globalObjects;
            _unrealReflection = unrealReflection;
            _sourceCodeGenerator = sourceCodeGenerator;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");
            ReflectAllMembers();
        }

        private unsafe void ReflectAllMembers() {
            var parentObjects = _globalObjects.EnumerateEverything();

            foreach (var parentObject in parentObjects) {
                var objectName = _globalObjects.GetNameString(parentObject.namePrivate.comparisonIndex);
                var objectClass = *parentObject.classPrivate;

                var properties = _unrealReflection.GetTypeProperties(objectClass);
                var fields = _unrealReflection.GetTypeFields(objectClass);

                if (properties.Count == 0) {
                    continue;
                }

                _logger.Debug($"Object: {objectName}");
                PrintMembers(fields, properties);
            }
        }

        private unsafe void PrintMembers(ICollection<UField> fields, ICollection<FField> properties) {
            var stringBuilder = new StringBuilder();
            foreach (var field in fields) {
                var fieldName = _globalObjects.GetNameString(field.baseUObject.namePrivate.comparisonIndex);
                var fieldFlags = Enum.GetName(field.baseUObject.objectFlags);
                stringBuilder.AppendLine($"Field: [{fieldFlags}] {fieldName}");
            }

            foreach (var property in properties) {
                var propertyName = _globalObjects.GetNameString(property.namePrivate.comparisonIndex);
                var properyFlags = Enum.GetName(property.flagsPrivate);
                stringBuilder.AppendLine($"Property: [{properyFlags}]{propertyName}");
            }

            _logger.Debug(stringBuilder.ToString());
        }

        public void Unload() {

        }
    }
}
