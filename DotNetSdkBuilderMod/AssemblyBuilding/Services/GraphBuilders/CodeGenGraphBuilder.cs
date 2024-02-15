using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class CodeGenGraphBuilder : ICodeGenGraphBuilder {
        private readonly ILogger _logger;
        private readonly INameDistanceService _nameDistanceService;
        private readonly IGlobalObjects _globalObjects;
        private readonly UnrealReflection _unrealReflection;

        public CodeGenGraphBuilder(ILogger logger, INameDistanceService nameDistanceService, IGlobalObjects globalObjects, UnrealReflection unrealReflection) {
            _logger = logger;
            _nameDistanceService = nameDistanceService;
            _globalObjects = globalObjects;
            _unrealReflection = unrealReflection;
        }

        // TODO: Fine tune this.
        // Do not make it user-configurable
        const int GROUPING_DISTANCE = 5;

        public CodeGenAssemblyNode[] BuildAssemblyGraphs(ClassNode rootNode) {
            DebugUtilities.WaitForDebuggerAttach();

            _logger.Debug("Getting distinct namespaces...");
            var distinctNamespaces = TimedDistinctNamespaces(rootNode);

            _logger.Debug("Building namespace tree...");
            var namespaceTree = TimedNamespaceTree(distinctNamespaces);

            _logger.Debug("Building namespace-namespace dictionary...");
            var memoizedNamespaceNodes = TimedNamespaceNamespaceNodeMemoize(namespaceTree);

            _logger.Debug("Building class-namespace dictionary...");
            var classNamespaces = TimedClassNamespaceMemoize(rootNode);

            _logger.Debug("Building namespace-classes dictionary...");
            var namespaceClasses = TimedNamespaceClassMemoize(rootNode);

            _logger.Debug("Applying classes to namespace tree...");
            TimedApplyClassesToNamespaces(namespaceClasses, memoizedNamespaceNodes);

            _logger.Debug("Applying classes to namespace tree...");
            TimedGenerateImports(namespaceTree, classNamespaces);

            var assemblyNodes = new CodeGenAssemblyNode[namespaceTree.Length];
            for (var i = 0; i < namespaceTree.Length; i++) {
                assemblyNodes[i] = new CodeGenAssemblyNode { namespaces = namespaceTree[i].namespaces, name = namespaceTree[i].nameSpace };
            }

            return assemblyNodes;
        }

        private string[] TimedDistinctNamespaces(ClassNode rootNode) {
            var timer = new Stopwatch();
            timer.Start();
            var distinctNamespaces = new HashSet<string>();
            GetAllNamespaces(rootNode, distinctNamespaces);
            timer.Stop();

            _logger.Debug($"Distinct namespace; {timer.ElapsedMilliseconds} ms to build.");
            return distinctNamespaces.ToArray();
        }

        private void GetAllNamespaces(ClassNode currentNode, HashSet<string> namespaces) {
            namespaces.Add(currentNode.packageName);

            foreach (var child in currentNode.children) {
                GetAllNamespaces(child, namespaces);
            }
        }

        private CodeGenNamespaceNode[] TimedNamespaceTree(string[] namespaces) {
            var timer = new Stopwatch();
            timer.Start();
            var namespaceTree = new CodeGenNamespaceNode();
            _ = BuildAllNamespaces(namespaces, namespaceTree);
            timer.Stop();

            _logger.Debug($"Built namespace tree; {timer.ElapsedMilliseconds} ms to build.");
            return namespaceTree.namespaces;
        }

        private CodeGenNamespaceNode BuildAllNamespaces(string[] namespaceNames, CodeGenNamespaceNode currentNode) {
            var rootLevel = namespaceNames
                .Where(i => i.Contains('/'))
                .Select(i => i.Split('/')[1])
                .Distinct()
                .ToArray();

            currentNode.namespaces = new CodeGenNamespaceNode[rootLevel.Length];
            for (var x = 0; x < rootLevel.Length; x++) {
                var branchNamespace = rootLevel[x];

                currentNode.namespaces[x] = new CodeGenNamespaceNode() { nameSpace = branchNamespace };

                var nameSpacePrefix = $"/{currentNode.namespaces[x].nameSpace}/";
                var childNamespaceNames = namespaceNames
                    .Where(i => i.StartsWith(nameSpacePrefix))
                    .Select(i => i.Substring(nameSpacePrefix.Length - 1))
                    .ToArray();

                if (childNamespaceNames.Length == 0 || childNamespaceNames is [""]) {
                    continue;
                }

                currentNode.namespaces[x] = BuildAllNamespaces(childNamespaceNames, currentNode.namespaces[x]);
            }

            return currentNode;
        }

        private Dictionary<string, CodeGenNamespaceNode> TimedNamespaceNamespaceNodeMemoize(CodeGenNamespaceNode[] namespaceTree) {
            var timer = new Stopwatch();
            timer.Start();
            var namespacesMemo = new Dictionary<string, CodeGenNamespaceNode>();
            foreach (var namespaceNode in namespaceTree) {
                MemoizeNamespaceNodes(namespaceNode, "", namespacesMemo);
            }

            timer.Stop();

            _logger.Debug($"Built namespace tree; {timer.ElapsedMilliseconds} ms to build.");
            return namespacesMemo;
        }

        private void MemoizeNamespaceNodes(CodeGenNamespaceNode currentNode, string previousNameSpace, Dictionary<string, CodeGenNamespaceNode> namespacesMemo) {
            var currentNamespace = $"{previousNameSpace}/{currentNode.nameSpace}";

            namespacesMemo[currentNamespace] = currentNode;

            if (currentNode.namespaces is null) {
                return;
            }

            foreach (var namespaceNode in currentNode.namespaces) {
                MemoizeNamespaceNodes(namespaceNode, currentNamespace, namespacesMemo);
            }
        }

        private Dictionary<string, string> TimedClassNamespaceMemoize(ClassNode rootNode) {
            var timer = new Stopwatch();
            timer.Start();
            var memoizedClassesAndNamespaces = new Dictionary<string, string>();
            MemoizeClassesAndNamespaces(rootNode, memoizedClassesAndNamespaces);
            timer.Stop();

            _logger.Debug($"Memoized classes and namespaces; {timer.ElapsedMilliseconds} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private void MemoizeClassesAndNamespaces(ClassNode currentNode, Dictionary<string, string> memo) {
            var className = _globalObjects.GetNameString(currentNode.ClassName);
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(memo, className, out var previouslyExisted);

            if (!previouslyExisted) {
                value = currentNode.packageName;
            }

            foreach (var child in currentNode.children) {
                MemoizeClassesAndNamespaces(child, memo);
            }
        }

        private Dictionary<string, List<ClassNode>> TimedNamespaceClassMemoize(ClassNode rootNode) {
            var timer = new Stopwatch();
            timer.Start();
            var memoizedClassesAndNamespaces = new Dictionary<string, List<ClassNode>>();
            MemoizeNamespacesAndClassNodes(rootNode, memoizedClassesAndNamespaces);
            timer.Stop();

            _logger.Debug($"Memoized namespaces and classes; {timer.ElapsedMilliseconds} ms to build.");
            return memoizedClassesAndNamespaces;
        }

        private void MemoizeNamespacesAndClassNodes(ClassNode currentNode, Dictionary<string, List<ClassNode>> memo) {
            var classPackageName = currentNode.packageName;
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(memo, classPackageName, out var previouslyExisted);

            if (!previouslyExisted) {
                value = new List<ClassNode>();
            }

            value!.Add(currentNode);

            foreach (var child in currentNode.children) {
                MemoizeNamespacesAndClassNodes(child, memo);
            }
        }

        private void TimedApplyClassesToNamespaces(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo) {
            var timer = new Stopwatch();
            timer.Start();
            ApplyClassesToNamespace(namespaceClassesMemo, namespacesMemo);
            timer.Stop();

            _logger.Debug($"Applied classes to namespace tree; {timer.ElapsedMilliseconds} ms to build.");
        }

        private void ApplyClassesToNamespace(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo) {
            foreach (var (nameSpace, classNodes) in namespaceClassesMemo) {
                var namespaceNode = namespacesMemo[nameSpace];
                var codeGenClassNodes = new CodeGenClassNode[classNodes.Count];

                for (var i = 0; i < classNodes.Count; i++) {
                    var currentClassNode = classNodes[i];
                    codeGenClassNodes[i] = GenerateCodeGenClassNode(currentClassNode);
                }

                namespaceNode.classes = codeGenClassNodes;
            }
        }

        private unsafe CodeGenClassNode GenerateCodeGenClassNode(ClassNode classNode) {
            var classProperties = classNode.properties;
            CodeGenPropertyNode[]? properties = null;
            if (classProperties.Length > 0) {
                properties = new CodeGenPropertyNode[classProperties.Length];
                for (var i = 0; i < classProperties.Length; i++) {
                    properties[i] = GenerateCodeGenPropertyNode(classProperties[i]);
                }
            }

            var classMethods = classNode.functions;
            CodeGenMethodNode[]? methods = null;
            if (classMethods.Length > 0) {
                methods = new CodeGenMethodNode[classMethods.Length];
                for (var i = 0; i < classMethods.Length; i++) {
                    methods[i] = GenerateCodeGenMethodNode(classMethods[i]);
                }
            }

            var className = _globalObjects.GetNameString(classNode.ClassName);

            string? attributes = null;

            var baseClass = classNode.nodeClass.baseUStruct.superStruct;
            string? baseClassName = null;
            if (baseClass != null) {
                baseClassName = _globalObjects.GetNameString(baseClass->ObjectName);
            }

            return new CodeGenClassNode {
                propertyNodes = properties,
                methodNodes = methods,
                modifer = CodeGenConstants.PUBLIC,
                name = className,
                attributes = attributes,
                baseType = baseClassName
            };
        }

        private unsafe CodeGenPropertyNode GenerateCodeGenPropertyNode(FProperty property) {
            var propertyName = _globalObjects.GetNameString(property.ObjectName);

            string? attributes = null;

            var returnType = _globalObjects.GetNameString(property.baseFField.classPrivate->ObjectName);

            return new CodeGenPropertyNode {
                modifer = CodeGenConstants.PUBLIC,
                name = propertyName,
                attributes = attributes,
                returnType = returnType,
                get = CodeGenConstants.GETTER,
                set = CodeGenConstants.SETTER,
            };
        }

        private unsafe CodeGenMethodNode GenerateCodeGenMethodNode(UFunction method) {
            var methodName = _globalObjects.GetNameString(method.baseUstruct.ObjectName);

            string? attributes = null;

            var signature = _unrealReflection.GetFunctionSignature(method);
            string returnType;
            if (signature.returnValue.HasValue) {
                var signatureName = signature.returnValue.Value.classPrivate->ObjectName;
                var signatureString = _globalObjects.GetNameString(signatureName);
                returnType = signatureString;
            }
            else {
                returnType = CodeGenConstants.VOID;
            }

            var functionParams = signature.parameters;
            (string type, string name)[]? methodArgs = null;
            if (functionParams.Length > 0) {
                methodArgs = new (string type, string name)[functionParams.Length];
                for (var i = 0; i < functionParams.Length; i++) {
                    var currentParam = functionParams[i];
                    var type = _globalObjects.GetNameString(currentParam.classPrivate->ObjectName);
                    var name = _globalObjects.GetNameString(currentParam.ObjectName);
                    methodArgs[i] = (type, name);
                }
            }

            return new CodeGenMethodNode {
                modifer = CodeGenConstants.PUBLIC,
                name = methodName,
                attributes = attributes,
                returnType = returnType,
                arguments = methodArgs
            };
        }

        private void TimedGenerateImports(CodeGenNamespaceNode[] namespaceTree, Dictionary<string, string> classNamespaces) {
            var timer = new Stopwatch();
            timer.Start();
            foreach (var namespaceNode in namespaceTree) {
                ApplyImportsToNamespace(namespaceNode, "", classNamespaces);
            }

            timer.Stop();

            _logger.Debug($"Applied imports to namespace tree; {timer.ElapsedMilliseconds} ms to build.");
        }

        private void ApplyImportsToNamespace(CodeGenNamespaceNode current, string previousNameSpace, Dictionary<string, string> classNamespaces) {
            var currentNamespace = $"{previousNameSpace}/{current.nameSpace}";

            var imports = new HashSet<string>();

            // Lord have mercy on my soul for the amount of indentation
            if (current.classes != null) {
                foreach (var classNode in current.classes) {
                    if (classNode.propertyNodes != null) {
                        foreach (var propertyNode in classNode.propertyNodes) {
                            TryAddClassAsImport(propertyNode.returnType);
                        }
                    }

                    if (classNode.methodNodes != null) {
                        foreach (var methodNode in classNode.methodNodes) {
                            var returnType = methodNode.returnType;
                            if (returnType != CodeGenConstants.VOID) {
                                TryAddClassAsImport(returnType);
                            }

                            if (methodNode.arguments != null) {
                                foreach (var arg in methodNode.arguments) {
                                    TryAddClassAsImport(arg.type);
                                }
                            }
                        }
                    }
                }
            }

            var importsBuilder = new StringBuilder();
            foreach (var import in imports) {
                importsBuilder.Append($"{CodeGenConstants.USING} ");
                importsBuilder.Append(import.AsSpan(1));
                importsBuilder.AppendLine(";");
            }

            current.imports = importsBuilder
                .Replace('/', '.')
                .ToString();

            if (current.namespaces != null) {
                foreach (var child in current.namespaces) {
                    ApplyImportsToNamespace(child, currentNamespace, classNamespaces);
                }
            }

            void TryAddClassAsImport(string className) {
                if (classNamespaces.TryGetValue(className, out var argNamespace)
                    && !currentNamespace.StartsWith(argNamespace)
                    && !argNamespace.StartsWith(current.nameSpace)) {
                    imports.Add(argNamespace);
                }
            }
        }
    }
}