using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly IUObjectFuncs _uObjectFuncs;
        private readonly UnrealReflection _unrealReflection;
        private readonly SourceCodeGenerator _sourceCodeGenerator;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger,
            IGlobalObjects globalObjects, IUObjectFuncs uObjectFuncs, UnrealReflection unrealReflection,
            SourceCodeGenerator sourceCodeGenerator) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _globalObjects = globalObjects;
            _uObjectFuncs = uObjectFuncs;
            _unrealReflection = unrealReflection;
            _sourceCodeGenerator = sourceCodeGenerator;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");
            ReflectAllMembers();
        }

        private unsafe void ReflectAllMembers() {
            var parentObjects = _globalObjects.EnumerateParents();
            var uniqueObjects = new HashSet<string>();

            foreach (var parentObject in parentObjects) {

                // var externalPackage = _uObjectFuncs.GetExternalPackage(&parentObject);
                //if (externalPackage is not null) {
                //    var packageId = externalPackage->packageId.id;
                //    _logger.Debug($"[{packageId}] 0x{(nint)externalPackage:X}");
                //}

                // _logger.Debug($"Method Call Package: {packageName}");


                var fullpath = GetFullPath(parentObject);
                var baseClassObject = parentObject.classPrivate->ClassDefaultObject->baseObjectBaseUtility.baseUObjectBase;
                var classObjectName = _globalObjects.GetNameString(baseClassObject.namePrivate.comparisonIndex);
                var objectName = _globalObjects.GetNameString(parentObject.namePrivate.comparisonIndex);


                var objectClass = *parentObject.classPrivate;
                var fields = _unrealReflection.GetTypeFields(objectClass);

                var flags = GetFlagNames(parentObject.objectFlags);

                //if (fields.Count > 0) {
                //    var fieldNames = PrintMembers(fields);
                //    uniqueObjects.Add($"{fullpath} - \n{fieldNames}");
                //}

                //uniqueObjects.Add($"0x{(nint)parentObject.classPrivate:X} {classObjectName}:{objectName}");

                uniqueObjects.Add($"{fullpath} [{string.Join(" | ", flags)}]");
                _logger.Debug($"[{fullpath}] privateClass: {classObjectName}, Object: {objectName}");
            }

            var filePath = Path.GetFullPath("ObjectMap.txt");
            File.WriteAllLines(filePath, uniqueObjects.OrderBy(i => i));
            _logger.Debug(filePath);
        }

        private unsafe string GetFullPath(UObjectBase uObjectBase) {
            var nameBuilder = new List<string>();
            for (UObjectBase* currentObj = &uObjectBase; currentObj is not null; currentObj = (UObjectBase*)currentObj->outerPrivate) {
                var baseName = _globalObjects.GetNameString(currentObj->namePrivate.comparisonIndex);
                nameBuilder.Add(baseName);
            }

            nameBuilder.Reverse();
            return string.Join(".", nameBuilder);
        }

        private unsafe string PrintMembers(ICollection<FField> fields) {
            var stringBuilder = new StringBuilder();
            foreach (var field in fields) {

                var className = _globalObjects.GetNameString(field.classPrivate->name.comparisonIndex);

                var flagValue = field.flagsPrivate;
                var fieldFlags = string.Join(", ", GetFlagNames(flagValue));

                var fieldName = _globalObjects.GetNameString(field.namePrivate.comparisonIndex);

                stringBuilder.AppendLine($"Field: [{className}] {fieldName}");
            }

            return stringBuilder.ToString();
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
