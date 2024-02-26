using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Attributes;
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

            // DebugUtilities.WaitForDebuggerAttach();

            var codeCompiler = _codeCompilerFactory.CreateCompiler();
            RegisterAdditionalAssemblies(codeCompiler);

            _logger.Debug("Generating SDK code...");
            TimedGenerateSdkCode(assemblyGraphs, codeCompiler);

            codeCompiler.Compile();
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

        private void RegisterAdditionalAssemblies(ICodeCompiler compiler) {
            var sdkBuilderAssembly = typeof(SdkBuilder).Assembly;
            compiler.RegisterExistingAssembly(sdkBuilderAssembly);

            var frameworkSdkAssembly = typeof(DetourAttribute).Assembly;
            compiler.RegisterExistingAssembly(frameworkSdkAssembly);

            var systemAssembly = typeof(string).Assembly;
            compiler.RegisterExistingAssembly(systemAssembly);

            var systemMemoryAssembly = typeof(MemoryPool<byte>).Assembly;
            compiler.RegisterExistingAssembly(systemMemoryAssembly);

            var systemRuntimeCompilerServicesAssembly = typeof(Unsafe).Assembly;
            compiler.RegisterExistingAssembly(systemRuntimeCompilerServicesAssembly);
        }

        private void TimedGenerateSdkCode(CodeGenAssemblyNode[] assemblyGraphs, ICodeCompiler codeCompiler) {
            var time = _functionTimingService.Execute(() => {
                foreach (var assemblyGraph in assemblyGraphs) {
                    foreach (var nameSpace in assemblyGraph.namespaces) {
                        TraverseNodes(nameSpace, codeCompiler, assemblyGraph.name);
                    }
                }
            });

            _logger.Debug($"Sdk code; {time.TotalMilliseconds:F1} ms to generate.");
        }

        private void TraverseNodes(CodeGenNamespaceNode namespaceNode, ICodeCompiler codeCompiler, string assemblyName) {
            var classFile = new StringBuilder();

            _fileGenerator.GenerateFile(classFile, namespaceNode);
            if (classFile.Length > 0) {
                codeCompiler.AppendFile(classFile, assemblyName, namespaceNode.fullName);
            }

            if (namespaceNode.namespaces is not null) {
                foreach (var node in namespaceNode.namespaces) {
                    TraverseNodes(node, codeCompiler, assemblyName);
                }
            }
        }
    }
}
