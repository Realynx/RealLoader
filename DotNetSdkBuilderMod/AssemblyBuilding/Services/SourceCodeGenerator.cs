using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services {
    public class SourceCodeGenerator : ISourceCodeGenerator {
        private readonly ILogger _logger;
        private readonly IFileGenerator _fileGenerator;
        private readonly IReflectedGraphBuilder _reflectedGraphBuilder;
        private readonly ICodeGenGraphBuilder _codeGenGraphBuilder;
        private readonly IFunctionTimingService _functionTimingService;

        public SourceCodeGenerator(ILogger logger, IFileGenerator fileGenerator, IReflectedGraphBuilder reflectedGraphBuilder,
            ICodeGenGraphBuilder codeGenGraphBuilder, IFunctionTimingService functionTimingService) {
            _logger = logger;
            _fileGenerator = fileGenerator;
            _reflectedGraphBuilder = reflectedGraphBuilder;
            _codeGenGraphBuilder = codeGenGraphBuilder;
            _functionTimingService = functionTimingService;
        }

        public unsafe void BuildSourceCode() {
            _logger.Debug("Building object tree...");
            var rootNode = TimeObjectTreeBuilder();

            _logger.Debug("Building assembly graphs...");
            var assemblyGraphs = TimeAssemblyGraphBuilder(rootNode);

            var path = Path.GetFullPath("GeneratedCode.cs");
            using var sw = File.CreateText(path);

            try {
                foreach (var assemblyGraph in assemblyGraphs) {
                    foreach (var nameSpace in assemblyGraph.namespaces) {
                        TraverseNodes(nameSpace, sw);
                    }
                }
            }
            finally {
                _logger.Debug($"Generated code written to {path}");
            }
        }

        private void TraverseNodes(CodeGenNamespaceNode namespaceNode, StreamWriter sw) {
            var classFile = new StringBuilder();

            _fileGenerator.GenerateFile(classFile, namespaceNode);
            // _logger.Debug(classFile.ToString());
            sw.WriteLine(classFile.ToString());

            if (namespaceNode.namespaces is not null) {
                foreach (var node in namespaceNode.namespaces) {
                    TraverseNodes(node, sw);
                }
            }
        }

        private ClassNode TimeObjectTreeBuilder() {
            var time = _functionTimingService.Execute(_reflectedGraphBuilder.BuildRootNode, out var treeGraph);

            _logger.Debug($"Root Object Graph; {time.TotalMilliseconds:F1} ms to build.");
            return treeGraph;
        }

        private CodeGenAssemblyNode[] TimeAssemblyGraphBuilder(ClassNode rootNode) {
            var time = _functionTimingService.Execute(() => _codeGenGraphBuilder.BuildAssemblyGraphs(rootNode), out var assemblyGraph);

            _logger.Debug($"Assembly Graph; {time.TotalMilliseconds:F1} ms to build.");
            return assemblyGraph;
        }
    }
}
