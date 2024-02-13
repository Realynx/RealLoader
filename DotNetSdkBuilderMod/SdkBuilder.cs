using System.Data.Common;
using System.Reflection.Metadata;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;
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
            Thread.Sleep(TimeSpan.FromSeconds(10));

            var parentObjects = _globalObjects.EnumerateEverything();
            var uniqueObjects = new HashSet<string>();

            var classNames = new HashSet<string>(0);

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
                var functions = _unrealReflection.GetTypeFunctions(objectClass);

                var flags = GetFlagNames(parentObject.objectFlags);


                //if (objectName == "Default__AnimLayerInterface") {
                //    DebugUtilities.WaitForDebuggerAttach();
                //}

                var feldStrings = PrintMembers(fields, functions);
                var classEntry = $"[Flags]\n{string.Join(", ", flags)}\n{classObjectName}\n[Fields]\n{string.Join("\n", feldStrings)}";
                classNames.Add(classEntry);


                var logEntry = $"[{fullpath}] privateClass: [0x{(nint)parentObject.classPrivate:X}]{classObjectName}, Object: {objectName}";
                uniqueObjects.Add(logEntry);
                //_logger.Debug(logEntry);
            }

            var filePath = Path.GetFullPath("ObjectMap.txt");
            File.WriteAllText(filePath, string.Join("\n", uniqueObjects.OrderBy(i => i)));

            filePath = Path.GetFullPath("ClassMap.txt");
            File.WriteAllText(filePath, string.Join("\n", classNames.OrderBy(i => i)));
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

        private unsafe string PrintMembers(ICollection<FProperty> fields, ICollection<UFunction> funcs) {
            var stringBuilder = new StringBuilder();
            foreach (var field in fields) {

                var className = _globalObjects.GetNameString(field.baseFField.classPrivate->name.comparisonIndex);
                var fieldName = _globalObjects.GetNameString(field.baseFField.namePrivate.comparisonIndex);

                var flagValue = field.baseFField.flagsPrivate;
                var fieldFlags = string.Join(", ", GetFlagNames(flagValue));


                stringBuilder.AppendLine($"[0x{field.offset_Internal:X}] Field: [{className}] {fieldName}");
            }

            foreach (var function in funcs) {
                var fieldName = _globalObjects.GetNameString(function.baseUstruct.baseUfield.baseUObject.namePrivate.comparisonIndex);
                var flagValue = function.functionFlags;
                var funcFlags = string.Join(", ", GetFlagNames(flagValue));
                var paramnum = function.numParams;

                var propertyString = new StringBuilder();
                for (FProperty* currentProp = function.firstPropertyToInit; currentProp is not null; currentProp = currentProp->propertyLinkNext) {
                    var propName = _globalObjects.GetNameString(currentProp->baseFField.namePrivate.comparisonIndex);
                    var className = _globalObjects.GetNameString(currentProp->baseFField.classPrivate->name.comparisonIndex);

                    propertyString.AppendLine($"    Local Struct Property: [{className}] {propName}");
                }

                var childrenString = new StringBuilder();
                var prefixString = new StringBuilder();
                var signature = _unrealReflection.GetFunctionSignature(function);

                if (signature.returnValue.HasValue) {
                    var returnValue = signature.returnValue.Value;

                    var propName = _globalObjects.GetNameString(returnValue.namePrivate.comparisonIndex);
                    var className = _globalObjects.GetNameString(returnValue.classPrivate->name.comparisonIndex);
                    prefixString.Append($"{className}");
                }
                else {
                    prefixString.Append($"void");
                }

                foreach (var parameter in signature.parameters) {
                    var propName = _globalObjects.GetNameString(parameter.namePrivate.comparisonIndex);
                    var className = _globalObjects.GetNameString(parameter.classPrivate->name.comparisonIndex);

                    childrenString.Append($"{className} {propName}, ");
                }

                if (!string.IsNullOrWhiteSpace(childrenString.ToString())) {
                    childrenString = childrenString.Remove(childrenString.Length - 2, 2);
                }

                //if (paramnum > 2) {
                //    DebugUtilities.WaitForDebuggerAttach();
                //}

                var methodPropertyStructs = string.IsNullOrWhiteSpace(propertyString.ToString()) ? string.Empty : $"\n{propertyString}";
                stringBuilder.AppendLine($"[{funcFlags}] Function ({paramnum}):\n   - {prefixString} {fieldName}({childrenString.ToString()}){methodPropertyStructs}");
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
