using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
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
            var parentObjects = _globalObjects.EnumerateObjects();

            foreach (var parentObject in parentObjects) {

                var objectName = _globalObjects.GetNameString(parentObject.namePrivate.comparisonIndex);
                var objectClass = *parentObject.classPrivate;

                var fields = _unrealReflection.GetTypeFields(objectClass);
                if (fields.Count < 1) {
                    continue;
                }

                _logger.Debug($"Object: {objectName}");
                PrintMembers(fields);
            }
        }

        private unsafe void PrintMembers(ICollection<FField> fields) {
            var stringBuilder = new StringBuilder();
            foreach (var field in fields) {

                var className = _globalObjects.GetNameString(field.classPrivate->name.comparisonIndex);

                var flagValue = field.flagsPrivate;
                var fieldFlags = string.Join(", ", GetFlagNames(flagValue));

                var fieldName = _globalObjects.GetNameString(field.namePrivate.comparisonIndex);

                stringBuilder.AppendLine($"Field: [{className}] {fieldName}");
            }

            _logger.Debug(stringBuilder.ToString());
        }

        static string[] GetFlagNames(Enum flags) {
            var flagNames = new List<string>();

            foreach (Enum value in Enum.GetValues(flags.GetType())) {
                if (flags.HasFlag(value)) {
                    if (!value.Equals(default(Enum))) {
                        flagNames.Add(Enum.GetName(flags.GetType(), value));
                    }
                }
            }

            return flagNames.ToArray();
        }

        public void Unload() {

        }
    }
}
