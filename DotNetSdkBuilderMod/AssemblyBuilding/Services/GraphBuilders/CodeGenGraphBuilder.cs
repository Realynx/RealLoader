using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models;
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

            var precompiledAssemblies = new[] {
                typeof(string).Assembly, typeof(MemoryPool<byte>).Assembly, typeof(UObjectInterop).Assembly, typeof(object).Assembly, typeof(Unsafe).Assembly
            };

            _logger.Debug("Building precompiled class-namespace dictionary...");
            var precompiledClassNamespaces = TimedMemoizePrecompiledTypeNamespaces(precompiledAssemblies);

            _logger.Debug("Counting function arguments...");
            var functionArgCounts = TimedCountFunctionArguments(rootNode);

            _logger.Debug("Building interop extensions...");
            TimedBuildInteropExtensions(namespaceTree, functionArgCounts);

            _logger.Debug("Applying imports...");
            TimedApplyImports(namespaceTree, customClassNamespaces, precompiledClassNamespaces);

            _logger.Debug("Qualifying base types...");
            TimedQualifyBaseTypes(namespaceTree, customClassNamespaces);

            var assemblyNodes = GenerateAssemblyNodes(namespaceTree);

            _logger.Debug("Building namespace-assembly dictionary...");
            var namespaceAssemblyMemo = TimedMemoizeAssemblyNamespaces(assemblyNodes, precompiledAssemblies);

            _logger.Debug("Computing assembly references...");
            TimedResolveAssemblyReferences(assemblyNodes, namespaceAssemblyMemo);

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
                namespaceTree.namespaces[^1] = _uObjectInteropExtensionsBuilder.GetScaffoldNamespaceNode(CODE_NAMESPACE);
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

                memoizedClassesAndNamespaces.Add(U_OBJECT_INTEROP_EXTENSIONS_CLASS_NAME, $"{CODE_NAMESPACE}{DOT}{U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE}");
            });

            _logger.Debug($"Memoized classes and namespaces; {time.TotalMilliseconds:F1} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private Dictionary<string, string> TimedMemoizePrecompiledTypeNamespaces(Assembly[] assemblies) {
            var memoizedClassesAndNamespaces = new Dictionary<string, string>();

            var time = _functionTimingService.Execute(() => {
                foreach (var assembly in assemblies) {
                    _nameSpaceService.MemoizeAssemblyTypeNamespaces(assembly, memoizedClassesAndNamespaces);
                }
            });

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

        private void TimedAddClassesToNamespaces(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo,
            Dictionary<EClassCastFlags, string> castFlagNames) {
            var time = _functionTimingService.Execute(() => PopulateNamespaceClasses(namespaceClassesMemo, namespacesMemo, castFlagNames));

            _logger.Debug($"Applied classes to namespace tree; {time.TotalMilliseconds:F1} ms to build.");
        }

        private void PopulateNamespaceClasses(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo,
            Dictionary<EClassCastFlags, string> castFlagNames) {
            foreach (var (nameSpace, classNodes) in namespaceClassesMemo) {
                var namespaceNode = namespacesMemo[nameSpace];
                var codeGenClassNodes = new CodeGenClassNode[classNodes.Count];

                var i = 0;
                foreach (var classNode in classNodes) {
                    var generatedClassNode = _classNodeFactory.GenerateCodeGenClassNode(classNode, castFlagNames);
                    codeGenClassNodes[i] = generatedClassNode;

                    if (generatedClassNode.name is "ClassProperty") {
                        i++;
                        Array.Resize(ref codeGenClassNodes, codeGenClassNodes.Length + 1);
                        codeGenClassNodes[i] = _classNodeFactory.GenerateCustomClass("ClassPtrProperty", generatedClassNode.name);
                    }

                    if (generatedClassNode.name is "ObjectProperty") {
                        i++;
                        Array.Resize(ref codeGenClassNodes, codeGenClassNodes.Length + 1);
                        codeGenClassNodes[i] = _classNodeFactory.GenerateCustomClass("FieldPathProperty", generatedClassNode.name);
                    }

                    i++;
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
            var interopNamespace = namespaceTree.Single(x => x.fullNamespace.Contains(CODE_GEN_INTEROP_NAMESPACE));
            var extensionsNamespace = interopNamespace.namespaces!.Single(x => x.fullNamespace.Contains(U_OBJECT_INTEROP_EXTENSIONS_NAMESPACE));
            var time = _functionTimingService.Execute(() => _uObjectInteropExtensionsBuilder.PopulateNamespaceNode(extensionsNamespace, functionArgCounts));

            _logger.Debug($"Built interop extension methods; {time.TotalMilliseconds:F1} ms to build.");
        }

        private void TimedApplyImports(CodeGenNamespaceNode[] namespaceTree, Dictionary<string, string> customClassNamespaces, Dictionary<string, string> dotnetClassNamespaces) {
            var time = _functionTimingService.Execute(() => {
                foreach (var namespaceNode in namespaceTree) {
                    _importResolver.ApplyImports(namespaceNode, customClassNamespaces, dotnetClassNamespaces);
                }
            });

            _logger.Debug($"Applied imports to namespace tree; {time.TotalMilliseconds:F1} ms to apply.");
        }

        private void TimedQualifyBaseTypes(CodeGenNamespaceNode[] namespaceTree, Dictionary<string, string> customClassNamespaces) {
            var time = _functionTimingService.Execute(() => {
                foreach (var namespaceNode in namespaceTree) {
                    QualifyBaseTypes(namespaceNode, customClassNamespaces);
                }
            });

            _logger.Debug($"Qualified base types; {time.TotalMilliseconds:F1} ms to apply.");
        }

        private void QualifyBaseTypes(CodeGenNamespaceNode current, Dictionary<string, string> customClassNamespaces) {
            if (current.classes is not null) {
                foreach (var classNode in current.classes) {
                    if (classNode.baseType is null || !customClassNamespaces.TryGetValue(classNode.baseType, out var baseTypeNamespace)) {
                        continue;
                    }

                    classNode.baseType = new StringBuilder(baseTypeNamespace)
                        .TrimStart('/')
                        .Replace('/', '.')
                        .Append('.')
                        .Append(classNode.baseType)
                        .ToString();
                }
            }

            if (current.namespaces is not null) {
                foreach (var namespaceNode in current.namespaces) {
                    QualifyBaseTypes(namespaceNode, customClassNamespaces);
                }
            }
        }

        private Dictionary<string, string> TimedMemoizeAssemblyNamespaces(CodeGenAssemblyNode[] assemblyNodes, Assembly[] precompiledAssemblies) {
            var time = _functionTimingService.Execute(() => {
                var memo = MemoizePrecompiledAssemblyNamespaces(precompiledAssemblies);

                foreach (var assemblyNode in assemblyNodes) {
                    foreach (var namespaceNode in assemblyNode.namespaces) {
                        MemoizeCustomAssemblyNamespaces(namespaceNode, assemblyNode.name, memo);
                    }
                }

                return memo;
            }, out var memo2);

            _logger.Debug($"Memoized assembly namespaces; {time.TotalMilliseconds:F1} ms to build.");
            return memo2;
        }

        private Dictionary<string, string> MemoizePrecompiledAssemblyNamespaces(IEnumerable<Assembly> assemblies) {
            var memo = new Dictionary<string, string>();
            foreach (var assembly in assemblies) {
                if (!File.Exists(assembly.Location)) {
                    _logger.Debug($"{assembly.GetName().FullName} does not exist on disk at {assembly.Location}.");
                    continue;
                }

                if (assembly.GetName().Name is "System.Private.CoreLib") {
                    continue;
                }

                var types = assembly.GetTypes();
                foreach (var type in types.Where(x => x.Namespace is not null)) {
                    memo.TryAdd(type.Namespace!, assembly.Location);
                }
            }

            return memo;
        }

        private void MemoizeCustomAssemblyNamespaces(CodeGenNamespaceNode currentNode, string currentAssembly, Dictionary<string, string> namespaces) {
            namespaces[currentNode.fullNamespace] = currentAssembly;

            if (currentNode.namespaces is not null) {
                foreach (var namespaceNode in currentNode.namespaces) {
                    MemoizeCustomAssemblyNamespaces(namespaceNode, currentAssembly, namespaces);
                }
            }
        }

        private CodeGenAssemblyNode[] GenerateAssemblyNodes(CodeGenNamespaceNode[] namespaceTree) {
            // TODO: Maybe we could separate by engine code and game code
            var assemblyNodes = new CodeGenAssemblyNode[1];
            assemblyNodes[0] = new CodeGenAssemblyNode {
                namespaces = namespaceTree,
                name = CODE_NAMESPACE,
                attributes = new[] {
                    _attributeNodeFactory.GenerateAssemblyAttribute(COMPATIBLE_GAME_VERSION_ATTRIBUTE, "0.1.2")
                }, // TODO: Get game version
            };

            return assemblyNodes;
        }

        private void TimedResolveAssemblyReferences(CodeGenAssemblyNode[] assemblyNodes, Dictionary<string, string> assemblyNamespaces) {
            var time = _functionTimingService.Execute(() => {
                foreach (var assemblyNode in assemblyNodes) {
                    var references = new HashSet<string>();
                    foreach (var namespaceNode in assemblyNode.namespaces) {
                        ResolveReferences(namespaceNode, assemblyNamespaces, references);
                    }

                    references.Remove(assemblyNode.name);
                    assemblyNode.references = references.Count > 0 ? references.ToArray() : null;
                }
            });

            _logger.Debug($"Resolved assembly references; {time.TotalMilliseconds:F1} ms to resolve.");
        }

        private void ResolveReferences(CodeGenNamespaceNode currentNode, Dictionary<string, string> assemblyNamespaces, HashSet<string> references) {
            if (currentNode.imports is not null) {
                foreach (var import in currentNode.imports) {
                    if (assemblyNamespaces.TryGetValue(import, out var reference)) {
                        references.Add(reference);
                    }
                }
            }

            if (currentNode.namespaces is not null) {
                foreach (var namespaceNode in currentNode.namespaces) {
                    ResolveReferences(namespaceNode, assemblyNamespaces, references);
                }
            }
        }
    }
}