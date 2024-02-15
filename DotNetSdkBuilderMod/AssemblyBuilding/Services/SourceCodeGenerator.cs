using System.Diagnostics;
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

        public SourceCodeGenerator(ILogger logger, IFileGenerator fileGenerator, IReflectedGraphBuilder reflectedGraphBuilder,
            ICodeGenGraphBuilder codeGenGraphBuilder) {
            _logger = logger;
            _fileGenerator = fileGenerator;
            _reflectedGraphBuilder = reflectedGraphBuilder;
            _codeGenGraphBuilder = codeGenGraphBuilder;
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
            sw.WriteLine("------------------------------");

            if (namespaceNode.namespaces is not null) {
                foreach (var node in namespaceNode.namespaces) {
                    TraverseNodes(node, sw);
                }
            }
        }

        private ClassNode TimeObjectTreeBuilder() {
            var timer = new Stopwatch();

            timer.Start();
            var treeGraph = _reflectedGraphBuilder.BuildRootNode();
            timer.Stop();

            if (treeGraph is null) {
                _logger.Error("Failed to build tree graph.");
            }

            _logger.Debug($"Root Object Graph; {timer.ElapsedMilliseconds} ms to build.");
            return treeGraph;
        }

        private CodeGenAssemblyNode[] TimeAssemblyGraphBuilder(ClassNode rootNode) {
            var timer = new Stopwatch();

            timer.Start();
            var assemblyGraph = _codeGenGraphBuilder.BuildAssemblyGraphs(rootNode);
            timer.Stop();

            _logger.Debug($"Assembly Graph; {timer.ElapsedMilliseconds} ms to build.");
            return assemblyGraph;
        }
    }
}
