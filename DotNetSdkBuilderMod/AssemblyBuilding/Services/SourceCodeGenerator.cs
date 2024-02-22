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
        private readonly ICodeCompilerFactory _codeCompilerFactory;

        public SourceCodeGenerator(ILogger logger, IFileGenerator fileGenerator, IReflectedGraphBuilder reflectedGraphBuilder,
            ICodeGenGraphBuilder codeGenGraphBuilder, IFunctionTimingService functionTimingService, ICodeCompilerFactory codeCompilerFactory) {
            _logger = logger;
            _fileGenerator = fileGenerator;
            _reflectedGraphBuilder = reflectedGraphBuilder;
            _codeGenGraphBuilder = codeGenGraphBuilder;
            _functionTimingService = functionTimingService;
            _codeCompilerFactory = codeCompilerFactory;
        }

        public unsafe void BuildSourceCode() {
            _logger.Debug("Building object tree...");
            var rootNode = TimeObjectTreeBuilder();

            _logger.Debug("Building assembly graphs...");
            var assemblyGraphs = TimeAssemblyGraphBuilder(rootNode);

            var compiler = _codeCompilerFactory.CreateCompiler();

            foreach (var assemblyGraph in assemblyGraphs) {
                foreach (var nameSpace in assemblyGraph.namespaces) {
                    TraverseNodes(nameSpace, compiler);
                }

                compiler.Compile(assemblyGraph.name);
            }
        }

        private void TraverseNodes(CodeGenNamespaceNode namespaceNode, ICodeCompiler codeCompiler) {
            var classFile = new StringBuilder();

            _fileGenerator.GenerateFile(classFile, namespaceNode);
            if (classFile.Length > 0) {
                codeCompiler.AppendFile(classFile, namespaceNode.packageName.TrimStart('/').Replace('/', '.'));
            }

            if (namespaceNode.namespaces is not null) {
                foreach (var node in namespaceNode.namespaces) {
                    TraverseNodes(node, codeCompiler);
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
