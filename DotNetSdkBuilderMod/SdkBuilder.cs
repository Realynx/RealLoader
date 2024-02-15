using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Interfaces;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod {
    [PalworldMod(nameof(SdkBuilder), "poofyfox", ".poofyfox", "1.0.0", PalworldModType.Universal)]
    public class SdkBuilder : IPalworldMod {
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private readonly ISourceCodeGenerator _sourceCodeGenerator;
        private readonly IGlobalObjects _globalObjects;
        private readonly IReflectedGraphBuilder _reflectedGraphBuilder;
        private readonly IUObjectFuncs _uObjectFuncs;

        public SdkBuilder(CancellationToken cancellationToken, ILogger logger, ISourceCodeGenerator sourceCodeGenerator,
            IGlobalObjects globalObjects, IReflectedGraphBuilder reflectedGraphBuilder, IUObjectFuncs uObjectFuncs) {
            _cancellationToken = cancellationToken;
            _logger = logger;
            _sourceCodeGenerator = sourceCodeGenerator;
            _globalObjects = globalObjects;
            _reflectedGraphBuilder = reflectedGraphBuilder;
            _uObjectFuncs = uObjectFuncs;
        }

        public unsafe void Load() {
            _logger.Debug("Loading SDK Builder!");

            // Need to wait for stuff to load into memory
            Thread.Sleep(TimeSpan.FromSeconds(5));

            //var rootNode = _reflectedGraphBuilder.BuildRootNode();
            //var tree = GenerateTree(rootNode!);
            //_logger.Debug(tree);

            //var filePath = Path.GetFullPath("packageTree.txt");
            //File.WriteAllText(filePath, tree);
            //_logger.Info(filePath);

            _sourceCodeGenerator.BuildSourceCode();
        }

        private unsafe string GenerateTree(ClassNode currentNode, int tabIndex = 0) {
            var packageName = GetPackageName(currentNode);

            var currentName = _globalObjects.GetNameString(currentNode.ClassName);
            var tree = $"{new string(' ', tabIndex * 4)}- [{packageName}] {currentName}\n";

            foreach (var child in currentNode.children) {
                tree += GenerateTree(child, tabIndex + 1);
            }
            return tree;
        }

        private unsafe string GetPackageName(ClassNode currentNode) {
            var nodeClassBase = currentNode.nodeClass.baseUStruct.baseUfield.baseUObject;

            var basePtr = (UObjectBaseUtility*)&nodeClassBase;
            var package = _uObjectFuncs.GetParentPackage(basePtr);

            var packageName = _globalObjects.GetNameString(package->Name);
            return packageName;
        }


        //private unsafe string GetFullPath(UObjectBase uObjectBase) {
        //    var nameBuilder = new List<string>();
        //    for (UObjectBase* currentObj = &uObjectBase; currentObj is not null; currentObj = (UObjectBase*)currentObj->outerPrivate) {
        //        var baseName = _globalObjects.GetNameString(currentObj->namePrivate.comparisonIndex);
        //        nameBuilder.Add(baseName);
        //    }

        //    nameBuilder.Reverse();
        //    return string.Join(".", nameBuilder);
        //}



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
