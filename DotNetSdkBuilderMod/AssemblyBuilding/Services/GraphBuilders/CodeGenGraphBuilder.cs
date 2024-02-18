using System.Reflection;
using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class CodeGenGraphBuilder : ICodeGenGraphBuilder {
        private readonly ILogger _logger;
        private readonly INameSpaceService _nameSpaceService;
        private readonly IImportResolver _importResolver;
        private readonly IFunctionTimingService _functionTimingService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;
        private readonly ICodeGenClassNodeFactory _classNodeFactory;

        public CodeGenGraphBuilder(ILogger logger, INameSpaceService nameSpaceService, IImportResolver importResolver,
            IFunctionTimingService functionTimingService, ICodeGenAttributeNodeFactory attributeNodeFactory, ICodeGenClassNodeFactory classNodeFactory) {
            _logger = logger;
            _nameSpaceService = nameSpaceService;
            _importResolver = importResolver;
            _functionTimingService = functionTimingService;
            _attributeNodeFactory = attributeNodeFactory;
            _classNodeFactory = classNodeFactory;
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
            var time = _functionTimingService.Execute(() => _nameSpaceService.BuildNamespaceTree(namespaces, string.Empty, namespaceTree));

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

            _logger.Debug($"Built namespace tree; {time.TotalMilliseconds:F1} ms to build.");
            return namespacesMemo;
        }

        private Dictionary<string, string> TimedMemoizeTypeNamespaces(ClassNode rootNode) {
            var memoizedClassesAndNamespaces = new Dictionary<string, string>();
            var time = _functionTimingService.Execute(() => _nameSpaceService.MemoizeTypeNamespaces(rootNode, memoizedClassesAndNamespaces));

            _logger.Debug($"Memoized classes and namespaces; {time.TotalMilliseconds:F1} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private Dictionary<string, string> TimedMemoizeDotnetTypeNamespaces() {
            var assembly = Assembly.GetAssembly(typeof(string))!;
            var time = _functionTimingService.Execute(() => _nameSpaceService.MemoizeAssemblyTypeNamespaces(assembly), out var memoizedClassesAndNamespaces);

            _logger.Debug($"Memoized dotnet classes and namespaces; {time.TotalMilliseconds:F1} ms to build.");
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