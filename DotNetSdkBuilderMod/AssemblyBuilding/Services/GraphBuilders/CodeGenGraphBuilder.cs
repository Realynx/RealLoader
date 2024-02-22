using System.Buffers;
using System.Reflection;
using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Attributes;
using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class CodeGenGraphBuilder : ICodeGenGraphBuilder {
        private readonly ILogger _logger;
        private readonly INameSpaceService _nameSpaceService;
        private readonly IImportResolver _importResolver;
        private readonly IFunctionTimingService _functionTimingService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;
        private readonly ICodeGenClassNodeFactory _classNodeFactory;
        private readonly IUnrealReflection _unrealReflection;
        private readonly IUObjectInteropExtensionsBuilder _uObjectInteropExtensionsBuilder;

        public CodeGenGraphBuilder(ILogger logger, INameSpaceService nameSpaceService, IImportResolver importResolver,
            IFunctionTimingService functionTimingService, ICodeGenAttributeNodeFactory attributeNodeFactory, ICodeGenClassNodeFactory classNodeFactory,
            IUnrealReflection unrealReflection, IUObjectInteropExtensionsBuilder uObjectInteropExtensionsBuilder) {
            _logger = logger;
            _nameSpaceService = nameSpaceService;
            _importResolver = importResolver;
            _functionTimingService = functionTimingService;
            _attributeNodeFactory = attributeNodeFactory;
            _classNodeFactory = classNodeFactory;
            _unrealReflection = unrealReflection;
            _uObjectInteropExtensionsBuilder = uObjectInteropExtensionsBuilder;
        }

        public CodeGenAssemblyNode[] BuildAssemblyGraphs(ClassNode rootNode) {
            _logger.Debug("Getting distinct namespaces...");
            var distinctNamespaces = TimedDistinctNamespaces(rootNode);

            _logger.Debug("Building namespace tree...");
            var namespaceTree = TimedBuildNamespaceTree(distinctNamespaces);

            _logger.Debug("Building namespace-namespace dictionary...");
            var memoizedNamespaceNodes = TimedMemoizeNamespaceTree(namespaceTree);

            _logger.Debug("Building namespace-classes dictionary...");
            var namespaceClasses = TimedNamespaceClassesMemoize(rootNode);

            _logger.Debug("Building class cast flag dictionary...");
            var castFlagNames = TimedCastFlagNameMemoize();

            _logger.Debug("Applying classes to namespace tree...");
            TimedAddClassesToNamespaces(namespaceClasses, memoizedNamespaceNodes, castFlagNames);

            _logger.Debug("Building class-namespace dictionary...");
            var customClassNamespaces = TimedMemoizeTypeNamespaces(rootNode);

            _logger.Debug("Building dotnet class-namespace dictionary...");
            var dotnetClassNamespaces = TimedMemoizeDotnetTypeNamespaces();

            _logger.Debug("Counting function arguments...");
            var functionArgCounts = TimedCountFunctionArguments(rootNode);

            _logger.Debug("Building interop extensions...");
            TimedBuildInteropExtensions(namespaceTree, functionArgCounts);

            _logger.Debug("Applying imports...");
            TimedApplyImports(namespaceTree, customClassNamespaces, dotnetClassNamespaces);

            var assemblyNodes = new CodeGenAssemblyNode[namespaceTree.Length];
            for (var i = 0; i < namespaceTree.Length; i++) {
                assemblyNodes[i] = new CodeGenAssemblyNode {
                    namespaces = namespaceTree[i].namespaces!,
                    name = namespaceTree[i].name,
                    attributes = new[] {
                        _attributeNodeFactory.GenerateAssemblyAttribute(COMPATIBLE_GAME_VERSION_ATTRIBUTE, "0.1.2")
                    }, // TODO: Get game version
                };
            }

            return assemblyNodes;
        }

        private string[] TimedDistinctNamespaces(ClassNode rootNode) {
            var distinctNamespaces = new HashSet<string>();
            var time = _functionTimingService.Execute(() => _nameSpaceService.GetUniqueNamespaces(rootNode, distinctNamespaces));

            _logger.Debug($"Distinct namespace; {time.TotalMilliseconds:F1} ms to build.");
            return distinctNamespaces.ToArray();
        }

        private CodeGenNamespaceNode[] TimedBuildNamespaceTree(string[] namespaces) {
            var namespaceTree = new CodeGenNamespaceNode();

            var time = _functionTimingService.Execute(() => {
                _nameSpaceService.BuildNamespaceTree(namespaces, string.Empty, namespaceTree);

                Array.Resize(ref namespaceTree.namespaces, namespaceTree.namespaces!.Length + 1);
                namespaceTree.namespaces[^1] = _uObjectInteropExtensionsBuilder.GetScaffoldNamespaceNode();
            });

            _logger.Debug($"Built namespace tree; {time.TotalMilliseconds:F1} ms to build.");
            return namespaceTree.namespaces!;
        }

        private Dictionary<string, CodeGenNamespaceNode> TimedMemoizeNamespaceTree(CodeGenNamespaceNode[] namespaceTree) {
            var namespacesMemo = new Dictionary<string, CodeGenNamespaceNode>();
            var time = _functionTimingService.Execute(() => {
                foreach (var namespaceNode in namespaceTree) {
                    _nameSpaceService.MemoizeNamespaceTree(namespaceNode, namespacesMemo);
                }
            });

            _logger.Debug($"Memoized namespace tree; {time.TotalMilliseconds:F1} ms to build.");
            return namespacesMemo;
        }

        private Dictionary<string, string> TimedMemoizeTypeNamespaces(ClassNode rootNode) {
            var memoizedClassesAndNamespaces = new Dictionary<string, string>();

            var time = _functionTimingService.Execute(() => {
                _nameSpaceService.MemoizeTypeNamespaces(rootNode, memoizedClassesAndNamespaces);

                var sdkBuilderAssembly = typeof(SdkBuilder).Assembly;
                _nameSpaceService.MemoizeAssemblyTypeNamespaces(sdkBuilderAssembly, memoizedClassesAndNamespaces);

                var frameworkSdkAssembly = typeof(DetourAttribute).Assembly;
                _nameSpaceService.MemoizeAssemblyTypeNamespaces(frameworkSdkAssembly, memoizedClassesAndNamespaces);

                memoizedClassesAndNamespaces.Add(U_OBJECT_INTEROP_EXTENSIONS_CLASS_NAME, U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE);
            });

            _logger.Debug($"Memoized classes and namespaces; {time.TotalMilliseconds:F1} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private Dictionary<string, string> TimedMemoizeDotnetTypeNamespaces() {
            var memoizedClassesAndNamespaces = new Dictionary<string, string>();

            var systemAssembly = Assembly.GetAssembly(typeof(string))!;
            var time1 = _functionTimingService.Execute(() => _nameSpaceService.MemoizeAssemblyTypeNamespaces(systemAssembly, memoizedClassesAndNamespaces));

            var systemBuffersAssembly = Assembly.GetAssembly(typeof(MemoryPool<byte>))!;
            var time2 = _functionTimingService.Execute(() => _nameSpaceService.MemoizeAssemblyTypeNamespaces(systemBuffersAssembly, memoizedClassesAndNamespaces));

            _logger.Debug($"Memoized dotnet classes and namespaces; {(time1 + time2).TotalMilliseconds:F1} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private Dictionary<string, List<ClassNode>> TimedNamespaceClassesMemoize(ClassNode rootNode) {
            var memoizedClassesAndNamespaces = new Dictionary<string, List<ClassNode>>();
            var time = _functionTimingService.Execute(() => MemoizeNamespacesClasses(rootNode, memoizedClassesAndNamespaces));

            _logger.Debug($"Memoized namespaces and classes; {time.TotalMilliseconds:F1} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private void MemoizeNamespacesClasses(ClassNode currentNode, Dictionary<string, List<ClassNode>> memo) {
            var classPackageName = currentNode.packageName;
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(memo, classPackageName, out var previouslyExisted);

            if (!previouslyExisted) {
                value = new List<ClassNode>();
            }

            value!.Add(currentNode);

            foreach (var child in currentNode.children) {
                MemoizeNamespacesClasses(child, memo);
            }
        }

        private Dictionary<EClassCastFlags, string> TimedCastFlagNameMemoize() {
            var castFlagNames = new Dictionary<EClassCastFlags, string>();
            var time = _functionTimingService.Execute(() => {
                var names = Enum.GetNames<EClassCastFlags>();
                var values = Enum.GetValues<EClassCastFlags>();

                // Skip the first value
                for (var i = 1; i < names.Length; i++) {
                    castFlagNames[values[i]] = names[i].Substring(11);
                }
            });

            _logger.Debug($"Memoized cast flag names; {time.TotalMilliseconds:F1} ms to build.");
            return castFlagNames;
        }

        private void TimedAddClassesToNamespaces(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo, Dictionary<EClassCastFlags, string> castFlagNames) {
            var time = _functionTimingService.Execute(() => PopulateNamespaceClasses(namespaceClassesMemo, namespacesMemo, castFlagNames));

            _logger.Debug($"Applied classes to namespace tree; {time.TotalMilliseconds:F1} ms to build.");
        }

        private void PopulateNamespaceClasses(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo, Dictionary<EClassCastFlags, string> castFlagNames) {
            foreach (var (nameSpace, classNodes) in namespaceClassesMemo) {
                var namespaceNode = namespacesMemo[nameSpace];
                var codeGenClassNodes = new CodeGenClassNode[classNodes.Count];

                for (var i = 0; i < classNodes.Count; i++) {
                    var currentClassNode = classNodes[i];
                    codeGenClassNodes[i] = _classNodeFactory.GenerateCodeGenClassNode(currentClassNode, castFlagNames);
                }

                namespaceNode.classes = codeGenClassNodes;
            }
        }

        private IReadOnlyList<int> TimedCountFunctionArguments(ClassNode rootNode) {
            var argCounts = new HashSet<int>();
            var time = _functionTimingService.Execute(() => CountFunctionArguments(rootNode, argCounts));

            _logger.Debug($"Applied imports to namespace tree; {time.TotalMilliseconds:F1} ms to build.");
            return argCounts.Order().ToArray();
        }

        private unsafe void CountFunctionArguments(ClassNode currentNode, HashSet<int> argCounts) {
            foreach (var function in currentNode.functions) {
                var functionSignature = _unrealReflection.GetFunctionSignature(function, out var returnValue, out _);
                var argumentsCount = functionSignature.Length + (returnValue is not null ? 1 : 0);
                argCounts.Add(argumentsCount);
            }

            foreach (var child in currentNode.children) {
                CountFunctionArguments(child, argCounts);
            }
        }

        private void TimedBuildInteropExtensions(CodeGenNamespaceNode[] namespaceTree, IReadOnlyList<int> functionArgCounts) {
            var interopNamespace = namespaceTree.First(x => x.packageName.Equals(CODE_GEN_INTEROP_NAMESPACE));
            var extensionsNamespace = interopNamespace.namespaces!.First(x => x.packageName.Equals(U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE));
            var time = _functionTimingService.Execute(() => _uObjectInteropExtensionsBuilder.PopulateNamespaceNode(extensionsNamespace, functionArgCounts));

            _logger.Debug($"Built interop extension methods; {time.TotalMilliseconds:F1} ms to build.");
        }

        private void TimedApplyImports(CodeGenNamespaceNode[] namespaceTree, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            var time = _functionTimingService.Execute(() => {
                foreach (var namespaceNode in namespaceTree) {
                    _importResolver.ApplyImports(namespaceNode, customClassNamespaces, dotnetClassNamespaces);
                }
            });

            _logger.Debug($"Applied imports to namespace tree; {time.TotalMilliseconds:F1} ms to build.");
        }
    }
}