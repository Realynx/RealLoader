using System.Diagnostics;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding;
using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

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
        private readonly IReflectedGraphBuilder _reflectedGraphBuilder;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger,
            IGlobalObjects globalObjects, IUObjectFuncs uObjectFuncs, UnrealReflection unrealReflection,
            SourceCodeGenerator sourceCodeGenerator, IReflectedGraphBuilder reflectedGraphBuilder) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _globalObjects = globalObjects;
            _uObjectFuncs = uObjectFuncs;
            _unrealReflection = unrealReflection;
            _sourceCodeGenerator = sourceCodeGenerator;
            _reflectedGraphBuilder = reflectedGraphBuilder;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");
            ReflectAllMembers();
        }

        private string GenerateTree(ClassNode currentNode, int tabIndex = 0) {
            var currentName = _globalObjects.GetNameString(currentNode.ClassName);
            var tree = $"{new string(' ', tabIndex * 4)}- {currentName}\n";

            foreach (var child in currentNode.children) {
                tree += GenerateTree(child, tabIndex + 1);
            }
            return tree;
        }

        private unsafe void ReflectAllMembers() {
            //Thread.Sleep(TimeSpan.FromSeconds(10));


            var typeGraph = _reflectedGraphBuilder.BuildRootNode();
            //DebugUtilities.WaitForDebuggerAttach();

            var timer = new Stopwatch();
            timer.Start();
            var tree = GenerateTree(typeGraph);
            timer.Stop();
            _logger.Debug(tree);

            _logger.Debug($"Graph took: {timer.ElapsedMilliseconds} MS to build.");

            var filePath = Path.GetFullPath("Inheritance.txt");
            File.WriteAllText(filePath, tree);

            //var parentObjects = _globalObjects.EnumerateEverything();
            //var uniqueObjects = new HashSet<string>();
            //var unLinkedNodes = new Dictionary<string, List<ClassNode>>();

            //var classNames = new HashSet<string>(0);

            //foreach (var parentObject in parentObjects) {

            // var externalPackage = _uObjectFuncs.GetExternalPackage(&parentObject);
            //if (externalPackage is not null) {
            //    var packageId = externalPackage->packageId.id;
            //    _logger.Debug($"[{packageId}] 0x{(nint)externalPackage:X}");
            //}

            // _logger.Debug($"Method Call Package: {packageName}");


            //var fullpath = GetFullPath(parentObject);
            //var baseClassObject = parentObject.classPrivate->ClassDefaultObject->baseObjectBaseUtility.baseUObjectBase;
            //var classObjectName = _globalObjects.GetNameString(baseClassObject.namePrivate.comparisonIndex);
            //var objectName = _globalObjects.GetNameString(parentObject.namePrivate.comparisonIndex);

            //var baseClass = parentObject.classPrivate->baseUStruct.superStruct;
            //var baseClassName = string.Empty;
            //if (baseClass is not null) {
            //    baseClassName = _globalObjects.GetNameString(baseClass->baseUfield.baseUObject.namePrivate.comparisonIndex);
            //}

            //var objectClass = *parentObject.classPrivate;
            //var fields = _unrealReflection.GetTypeFields(objectClass);
            //var functions = _unrealReflection.GetTypeFunctions(objectClass);

            //var flags = GetFlagNames(parentObject.objectFlags);


            //if (objectName == "Default__AnimLayerInterface") {
            //    DebugUtilities.WaitForDebuggerAttach();
            //}

            //var feldStrings = PrintMembers(fields, functions);
            //var classEntry = $"[Flags]\n{string.Join(", ", flags)}\n{classObjectName} : {baseClassName}\n[Fields]\n{string.Join("\n", feldStrings)}";
            //classNames.Add(classEntry);


            //var logEntry = $"[{fullpath}] privateClass: [0x{(nint)parentObject.classPrivate:X}]{classObjectName}, Object: {objectName}";
            //uniqueObjects.Add(logEntry);
            ////_logger.Debug(logEntry);

            //// Add our node
            //var currentNode = new ClassNode() {
            //    functions = functions.ToArray(),
            //    properties = fields.ToArray(),
            //    nodeClass = *parentObject.classPrivate
            //};
            //unLinkedNodes.Add(classObjectName, new List<ClassNode>() { currentNode });
            //}

            //var filePath = Path.GetFullPath("ObjectMap.txt");
            //File.WriteAllText(filePath, string.Join("\n", uniqueObjects.OrderBy(i => i)));

            //filePath = Path.GetFullPath("ClassMap.txt");
            //File.WriteAllText(filePath, string.Join("\n", classNames.OrderBy(i => i)));
            //_logger.Debug(filePath);

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
