using System.Diagnostics;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.GNameStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class SourceCodeGenerator : ISourceCodeGenerator {
        private readonly ILogger _logger;
        private readonly IFileGenerator _fileGenerator;
        private readonly IReflectedGraphBuilder _reflectedGraphBuilder;
        private readonly IGlobalObjects _globalObjects;
        private readonly IUObjectFuncs _uObjectFuncs;
        private readonly NameSpaceGenerator _nameSpaceGenerator;

        public SourceCodeGenerator(ILogger logger, IFileGenerator fileGenerator, IReflectedGraphBuilder reflectedGraphBuilder,
            IGlobalObjects globalObjects, IUObjectFuncs uObjectFuncs, NameSpaceGenerator nameSpaceGenerator) {
            _logger = logger;
            _fileGenerator = fileGenerator;
            _reflectedGraphBuilder = reflectedGraphBuilder;
            _globalObjects = globalObjects;
            _uObjectFuncs = uObjectFuncs;
            _nameSpaceGenerator = nameSpaceGenerator;
        }

        public unsafe void BuildSourceCode() {
            _logger.Debug("Building object tree...");
            var rootNode = TimeGraphBuilder();
            TraverseNodes(rootNode);
        }

        private void TraverseNodes(ClassNode currentNode) {
            var packageName = GetPackageName(currentNode);

            var nameSpace = new StringBuilder(packageName)
                .Replace('/', '.')
                .Insert(0, "BaseNameSpace")
                .ToString();

            var classFile = new StringBuilder();
            var nameSpace = namespaces[currentNode.ClassName];
            DebugUtilities.WaitForDebuggerAttach();
            _fileGenerator.GenerateFile(classFile, currentNode, nameSpace);

            _logger.Debug(classFile.ToString());

            foreach (var node in currentNode.children) {
                TraverseNodes(node);
            }
        }

        private ClassNode TimeGraphBuilder() {
            var timer = new Stopwatch();

            timer.Start();
            var treeGraph = _reflectedGraphBuilder.BuildRootNode();
            timer.Stop();

            _logger.Debug($"Root Object Graph; {timer.ElapsedMilliseconds} ms to build.");
            return treeGraph;
        }

        /// <summary>
        /// Keep this for debugging, print out the graph.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="tabIndex"></param>
        /// <returns></returns>
        private string GenerateTree(ClassNode currentNode, int tabIndex = 0) {
            var currentName = _globalObjects.GetNameString(currentNode.ClassName);
            var tree = $"{new string(' ', tabIndex * 4)}- {currentName}\n";

            foreach (var child in currentNode.children) {
                tree += GenerateTree(child, tabIndex + 1);
            }
            return tree;
        }
    }
}
