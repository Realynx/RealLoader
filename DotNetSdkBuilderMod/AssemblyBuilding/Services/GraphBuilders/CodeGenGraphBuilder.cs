﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.Flags;
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

            _logger.Debug("Building class cast flag dictionary...");
            var castFlagNames = TimedCastFlagNameMemoize();

            _logger.Debug("Applying classes to namespace tree...");
            TimedAddClassesToNamespaces(namespaceClasses, memoizedNamespaceNodes, castFlagNames);

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
                    codeGenClassNodes[i] = GenerateCodeGenClassNode(currentClassNode, castFlagNames);
                }

                namespaceNode.classes = codeGenClassNodes;
            }
        }

        private unsafe CodeGenClassNode GenerateCodeGenClassNode(ClassNode classNode, Dictionary<EClassCastFlags, string> castFlagNames) {
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

            var modifiers = new StringBuilder(PUBLIC);
            if (properties is not null || methods is not null) {
                modifiers.Append($"{WHITE_SPACE}{UNSAFE}");
            }
            if (classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Abstract)) {
                modifiers.Append($"{WHITE_SPACE}{ABSTRACT}");
            }

            var className = _namePoolService.GetNameString(classNode.ClassName).Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

            var attributes = new List<string> { GenerateAttribute(FULLY_QUALIFIED_TYPE_PATH_ATTRIBUTE, $"{QUOTE}{classNode.packageName}/{className}{QUOTE}") };
            if (classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Deprecated)) {
                attributes.Add(GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var baseClass = classNode.nodeClass->baseUStruct.superStruct;
            string? baseClassName = null;
            if (baseClass is not null) {
                baseClassName = _namePoolService.GetNameString(baseClass->ObjectName);
            }

            CodeGenOperatorNode[]? operators = null;
            if (classNode.nodeClass->ClassCastFlags is not EClassCastFlags.CASTCLASS_None) {
                var canCastTo = new List<string>();

                var classCastFlags = (ulong)classNode.nodeClass->ClassCastFlags;
                var castFlagsEnd = (ulong)EClassCastFlags.CASTCLASS_FLargeWorldCoordinatesRealProperty;
                for (ulong i = 1; i < castFlagsEnd; i <<= 1) {
                    if ((classCastFlags & i) == i) {
                        var castClass = castFlagNames[(EClassCastFlags)i];
                        if (!IsClassOrBaseClass(castClass, classNode.nodeClass->baseUStruct)) {
                            canCastTo.Add(castClass);
                        }
                    }
                }

                operators = new CodeGenOperatorNode[canCastTo.Count];
                for (var i = 0; i < canCastTo.Count; i++) {
                    operators[i] = GenerateCastOperator(canCastTo[i], className);
                }
            }

            return new CodeGenClassNode {
                propertyNodes = properties,
                methodNodes = methods,
                modifier = modifiers.ToString(),
                name = className,
                attributes = attributes.ToArray(),
                baseType = baseClassName,
                operators = operators,
            };
        }

        private unsafe bool IsClassOrBaseClass(string otherClasName, UStruct uStruct) {
            var structName = _namePoolService.GetNameString(uStruct.ObjectName);
            if (structName == otherClasName) {
                return true;
            }

            var baseClass = uStruct.superStruct;
            if (baseClass is null) {
                return false;
            }

            return IsClassOrBaseClass(otherClasName, *baseClass);
        }

        private unsafe CodeGenPropertyNode GenerateCodeGenPropertyNode(FProperty* property) {
            var propertyName = _namePoolService.GetNameString(property->ObjectName).Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

            string[]? attributes = null;
            if (property->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes = new[] { GenerateAttribute(DEPRECATED_ATTRIBUTE) };
            }

            var returnType = _namePoolService.GetNameString(property->baseFField.classPrivate->ObjectName);

            var fieldOffset = property->offset_Internal;

            var getter = $"{GET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{STAR}{OPEN_ROUND_BRACKET}{returnType}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{SEMICOLON}";
            var setter = $"{SET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{STAR}{OPEN_ROUND_BRACKET}{returnType}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{VALUE}{SEMICOLON}";

            return new CodeGenPropertyNode {
                modifier = PUBLIC,
                name = propertyName,
                attributes = attributes,
                returnType = returnType,
                get = getter,
                set = setter,
            };
        }

        private unsafe CodeGenMethodNode GenerateCodeGenMethodNode(UFunction* method) {
            string modifiers;
            if (method->functionFlags.HasFlag(EFunctionFlags.FUNC_Static)) {
                modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}";
            }
            else {
                modifiers = $"{PUBLIC}";
            }

            var methodName = _namePoolService.GetNameString(method->baseUstruct.ObjectName).Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

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
                modifier = modifiers,
                name = methodName,
                attributes = attributes,
                returnType = returnType,
                arguments = methodArgs
            };
        }

        private CodeGenOperatorNode GenerateCastOperator(string canCastTo, string className) {
            var modifiers = $"{PUBLIC}{WHITE_SPACE}{STATIC}{WHITE_SPACE}{EXPLICIT}{WHITE_SPACE}{OPERATOR}";
            var result = $"{NEW}{WHITE_SPACE}{canCastTo}{OPEN_ROUND_BRACKET}{OPERATOR_THIS_CLASS_NAME}{DOT}{ADDRESS_FIELD_NAME}{CLOSED_ROUND_BRACKET}";

            return new CodeGenOperatorNode {
                name = className,
                modifier = modifiers,
                returnType = canCastTo,
                attributes = null,
                result = result,
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