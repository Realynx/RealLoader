﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FLags;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.GraphBuilders {
    public class CodeGenGraphBuilder : ICodeGenGraphBuilder {
        private readonly ILogger _logger;
        private readonly IUnrealReflection _unrealReflection;
        private readonly INameSpaceService _nameSpaceService;
        private readonly IImportResolver _importResolver;
        private readonly IFunctionTimingService _functionTimingService;
        private readonly INamePoolService _namePoolService;

        public CodeGenGraphBuilder(ILogger logger, IUnrealReflection unrealReflection, INameSpaceService nameSpaceService,
            IImportResolver importResolver, IFunctionTimingService functionTimingService, INamePoolService namePoolService) {
            _logger = logger;
            _unrealReflection = unrealReflection;
            _nameSpaceService = nameSpaceService;
            _importResolver = importResolver;
            _functionTimingService = functionTimingService;
            _namePoolService = namePoolService;
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

            _logger.Debug("Applying classes to namespace tree...");
            TimedAddClassesToNamespaces(namespaceClasses, memoizedNamespaceNodes);

            _logger.Debug("Building class-namespace dictionary...");
            var classNamespaces = TimedMemoizeTypeNamespaces(rootNode);

            _logger.Debug("Applying imports...");
            TimedApplyImports(namespaceTree, classNamespaces);

            var assemblyNodes = new CodeGenAssemblyNode[namespaceTree.Length];
            for (var i = 0; i < namespaceTree.Length; i++) {
                assemblyNodes[i] = new CodeGenAssemblyNode {
                    namespaces = namespaceTree[i].namespaces!,
                    name = namespaceTree[i].name,
                    attributes = new[] { GenerateAssemblyAttribute(COMPATIBLE_GAME_VERSION_ATTRIBUTE, "0.1.2") }, // TODO: Get game version
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

        private void TimedAddClassesToNamespaces(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo) {
            var time = _functionTimingService.Execute(() => PopulateNamespaceClasses(namespaceClassesMemo, namespacesMemo));

            _logger.Debug($"Applied classes to namespace tree; {time.TotalMilliseconds:F1} ms to build.");
        }

        private void PopulateNamespaceClasses(Dictionary<string, List<ClassNode>> namespaceClassesMemo, Dictionary<string, CodeGenNamespaceNode> namespacesMemo) {
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

            string modifiers;
            if (properties is not null || methods is not null) {
                modifiers = $"{PUBLIC}{WHITE_SPACE}{UNSAFE}";
            }
            else {
                modifiers = $"{PUBLIC}";
            }

            var className = _namePoolService.GetNameString(classNode.ClassName).Replace(" ", "-", StringComparison.InvariantCultureIgnoreCase);

            var attributes = new List<string> { GenerateAttribute(FULLY_QUALIFIED_TYPE_PATH_ATTRIBUTE, $"{QUOTE}{classNode.packageName}/{className}{QUOTE}") };
            if (classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Deprecated)) {
                attributes.Add(GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var baseClass = classNode.nodeClass->baseUStruct.superStruct;
            string? baseClassName = null;
            if (baseClass is not null) {
                baseClassName = _namePoolService.GetNameString(baseClass->ObjectName);
            }

            return new CodeGenClassNode {
                propertyNodes = properties,
                methodNodes = methods,
                modifer = modifiers,
                name = className,
                attributes = attributes.ToArray(),
                baseType = baseClassName
            };
        }

        private unsafe CodeGenPropertyNode GenerateCodeGenPropertyNode(FProperty* property) {
            var propertyName = _namePoolService.GetNameString(property->ObjectName).Replace(" ", "-", StringComparison.InvariantCultureIgnoreCase);

            string[]? attributes = null;
            if (property->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes = new[] { GenerateAttribute(DEPRECATED_ATTRIBUTE) };
            }

            var returnType = _namePoolService.GetNameString(property->baseFField.classPrivate->ObjectName);

            var fieldOffset = property->offset_Internal;

            var getter = $"{GET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{STAR}{OPEN_ROUND_BRACKET}{returnType}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{SEMICOLON}";
            var setter = $"{SET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{STAR}{OPEN_ROUND_BRACKET}{returnType}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{VALUE}{SEMICOLON}";

            return new CodeGenPropertyNode {
                modifer = PUBLIC,
                name = propertyName,
                attributes = attributes,
                returnType = returnType,
                get = getter,
                set = setter,
            };
        }

        private unsafe CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method) {
            var methodName = _namePoolService.GetNameString(method->baseUstruct.ObjectName).Replace(" ", "-", StringComparison.InvariantCultureIgnoreCase);

            string[]? attributes = null;

            var parameters = _unrealReflection.GetFunctionSignature(method, out var returnValue);
            string returnType;
            if (returnValue is not null) {
                var signatureName = returnValue->classPrivate->ObjectName;
                var signatureString = _namePoolService.GetNameString(signatureName);
                returnType = signatureString;
            }
            else {
                returnType = VOID;
            }

            (string type, string name)[]? methodArgs = null;
            if (parameters.Length > 0) {
                methodArgs = new (string type, string name)[parameters.Length];
                for (var i = 0; i < parameters.Length; i++) {
                    var currentParam = parameters[i];
                    var type = _namePoolService.GetNameString(currentParam->classPrivate->ObjectName);
                    var name = _namePoolService.GetNameString(currentParam->ObjectName);
                    methodArgs[i] = (type, name);
                }
            }

            return new CodeGenMethodNode {
                modifer = PUBLIC,
                name = methodName,
                attributes = attributes,
                returnType = returnType,
                arguments = methodArgs
            };
        }

        private string GenerateAssemblyAttribute([ConstantExpected] string attributeName, string attributeValue) {
            return $"{OPEN_SQUARE_BRACKET}{ASSEMBLY}{COLON}{WHITE_SPACE}{attributeName}{OPEN_ROUND_BRACKET}{attributeValue}{CLOSED_ROUND_BRACKET}{CLOSED_SQUARE_BRACKET}";
        }

        private string GenerateAttribute([ConstantExpected] string attributeName, string attributeValue) {
            return $"{OPEN_SQUARE_BRACKET}{attributeName}{OPEN_ROUND_BRACKET}{attributeValue}{CLOSED_ROUND_BRACKET}{CLOSED_SQUARE_BRACKET}";
        }

        private string GenerateAttribute([ConstantExpected] string attributeName) {
            return $"{OPEN_SQUARE_BRACKET}{attributeName}{CLOSED_SQUARE_BRACKET}";
        }

        private void TimedApplyImports(CodeGenNamespaceNode[] namespaceTree, Dictionary<string, string> classNamespaces) {
            var time = _functionTimingService.Execute(() => {
                foreach (var namespaceNode in namespaceTree) {
                    _importResolver.ApplyImports(namespaceNode, classNamespaces);
                }
            });

            _logger.Debug($"Applied imports to namespace tree; {time.TotalMilliseconds:F1} ms to build.");
        }
    }
}