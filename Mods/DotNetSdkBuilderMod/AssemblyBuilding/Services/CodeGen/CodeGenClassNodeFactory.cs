using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Models;
using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;
using RealLoaderFramework.Sdk.Models.CoreUObject.UClassStructs;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenClassNodeFactory : ICodeGenClassNodeFactory {
        private readonly INamePoolService _namePoolService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;
        private readonly ICodeGenConstructorNodeFactory _constructorNodeFactory;
        private readonly ICodeGenPropertyNodeFactory _propertyNodeFactory;
        private readonly ICodeGenMethodNodeFactory _methodNodeFactory;
        private readonly ICodeGenOperatorNodeFactory _operatorNodeFactory;
        private readonly ICodeGenInterfaceNodeFactory _interfaceNodeFactory;
        private readonly INameCollisionService _nameCollisionService;
        private readonly IUnrealReflection _unrealReflection;

        public CodeGenClassNodeFactory(INamePoolService namePoolService, ICodeGenAttributeNodeFactory attributeNodeFactory, ICodeGenConstructorNodeFactory constructorNodeFactory,
            ICodeGenPropertyNodeFactory propertyNodeFactory, ICodeGenMethodNodeFactory methodNodeFactory, ICodeGenOperatorNodeFactory operatorNodeFactory,
            ICodeGenInterfaceNodeFactory interfaceNodeFactory, INameCollisionService nameCollisionService, IUnrealReflection unrealReflection) {
            _namePoolService = namePoolService;
            _attributeNodeFactory = attributeNodeFactory;
            _constructorNodeFactory = constructorNodeFactory;
            _propertyNodeFactory = propertyNodeFactory;
            _methodNodeFactory = methodNodeFactory;
            _operatorNodeFactory = operatorNodeFactory;
            _interfaceNodeFactory = interfaceNodeFactory;
            _nameCollisionService = nameCollisionService;
            _unrealReflection = unrealReflection;
        }

        public unsafe CodeGenClassNode GenerateCodeGenClassNode(ClassNode classNode, Dictionary<EClassCastFlags, string> castFlagNames) {
            var nonSanitizedClassName = _namePoolService.GetNameString(classNode.ClassName);
            var className = _namePoolService.GetSanitizedNameString(classNode.ClassName);

            CodeGenConstructorNode[]? classConstructors = null;
            // TODO: Gather constructors
            if (true) {
                var constructors = new List<CodeGenConstructorNode> {
                    _constructorNodeFactory.GenerateDefaultConstructor(className)
                };
                // constructors.Add(_constructorNodeFactory.GenerateCodeGenConstructorNode());
                classConstructors = constructors.ToArray();
            }

            List<CodeGenPropertyNode>? properties = null;
            if (classNode.properties.Length > 0) {
                properties = new List<CodeGenPropertyNode>(classNode.properties.Length);
                var propertyNames = new HashSet<string> { className };

                for (var i = 0; i < classNode.properties.Length; i++) {
                    var currentProperty = classNode.properties[i];

                    CodeGenPropertyNode propertyNode;
                    if (currentProperty.inheritedFrom is null) {
                        propertyNode = _propertyNodeFactory.GenerateOwnedPropertyNode(currentProperty);
                    }
                    else {
                        propertyNode = _propertyNodeFactory.GenerateInheritedPropertyNode(currentProperty);
                    }

                    propertyNode.name = _nameCollisionService.GetNonCollidingName(propertyNode.name, propertyNames);
                    properties.Add(propertyNode);
                }
            }

            List<CodeGenMethodNode>? methods = null;
            if (classNode.functions.Length > 0) {
                methods = new List<CodeGenMethodNode>(classNode.functions.Length);

                for (var i = 0; i < classNode.functions.Length; i++) {
                    var currentFunction = classNode.functions[i];

                    CodeGenMethodNode methodNode;
                    if (currentFunction.inheritedFrom is null) {
                        methodNode = _methodNodeFactory.GenerateOwnedMethodNode(currentFunction, i);
                    }
                    else {
                        methodNode = _methodNodeFactory.GenerateInheritedMethodNode(currentFunction);
                    }

                    methodNode.name = _nameCollisionService.GetNonCollidingName(methodNode.name, className);
                    methods.Add(methodNode);
                }
            }

            var modifiers = new StringBuilder(PUBLIC);
            if (classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Abstract)) {
                modifiers.Append($"{WHITE_SPACE}{ABSTRACT}");
            }

            // Preserve modifier order
            modifiers.Append($"{WHITE_SPACE}{UNSAFE}");

            var attributes = new List<CodeGenAttributeNode> {
                _attributeNodeFactory.GenerateAttribute(FULLY_QUALIFIED_TYPE_PATH_ATTRIBUTE, $"{QUOTE}{classNode.packageName}/{className}{QUOTE}"),
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_TYPE_NAME_ATTRIBUTE, $"{QUOTE}{nonSanitizedClassName}{QUOTE}")
            };
            if (classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Deprecated)) {
                attributes.Add(_attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var baseClass = classNode.nodeClass->baseUStruct.superStruct;
            string? baseClassName;
            if (baseClass is not null) {
                baseClassName = _namePoolService.GetSanitizedNameString(baseClass->ObjectName);
            }
            else {
                baseClassName = nameof(UObjectInterop);
            }

            CodeGenInterfaceNode[]? interfaces = null;
            if (!classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Abstract)) {
                interfaces = new[] {
                    _interfaceNodeFactory.GenerateICreatableUObject(className)
                };
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

                if (canCastTo.Count > 0) {
                    operators = new CodeGenOperatorNode[canCastTo.Count];
                    for (var i = 0; i < canCastTo.Count; i++) {
                        operators[i] = _operatorNodeFactory.GenerateCastOperator(canCastTo[i], className);
                    }
                }
            }

            return new CodeGenClassNode {
                constructorNodes = classConstructors,
                propertyNodes = properties?.ToArray(),
                methodNodes = methods?.ToArray(),
                modifier = modifiers.ToString(),
                name = className,
                attributes = attributes.ToArray(),
                baseType = baseClassName,
                interfaces = interfaces,
                operatorNodes = operators,
            };
        }

        private unsafe bool IsClassOrBaseClass(string otherClasName, UStruct uStruct) {
            var structName = _namePoolService.GetSanitizedNameString(uStruct.ObjectName);
            if (structName == otherClasName) {
                return true;
            }

            var baseClass = uStruct.superStruct;
            if (baseClass is null) {
                return false;
            }

            return IsClassOrBaseClass(otherClasName, *baseClass);
        }

        public CodeGenClassNode GenerateCustomClass(string name, string baseType) {
            var constructors = new[] {
                _constructorNodeFactory.GenerateDefaultConstructor(name)
            };

            var modifiers = $"{PUBLIC}{WHITE_SPACE}{UNSAFE}";

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            var baseClassName = string.IsNullOrWhiteSpace(baseType) ? null : baseType;

            return new CodeGenClassNode {
                constructorNodes = constructors,
                modifier = modifiers,
                name = name,
                attributes = attributes,
                baseType = baseClassName,
            };
        }
    }
}