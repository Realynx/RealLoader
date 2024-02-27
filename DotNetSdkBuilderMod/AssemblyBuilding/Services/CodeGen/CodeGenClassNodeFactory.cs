using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenClassNodeFactory : ICodeGenClassNodeFactory {
        private readonly INamePoolService _namePoolService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;
        private readonly ICodeGenConstructorNodeFactory _constructorNodeFactory;
        private readonly ICodeGenPropertyNodeFactory _propertyNodeFactory;
        private readonly ICodeGenMethodNodeFactory _methodNodeFactory;
        private readonly ICodeGenOperatorNodeFactory _operatorNodeFactory;
        private readonly INameCollisionService _nameCollisionService;

        public CodeGenClassNodeFactory(INamePoolService namePoolService, ICodeGenAttributeNodeFactory attributeNodeFactory, ICodeGenConstructorNodeFactory constructorNodeFactory,
            ICodeGenPropertyNodeFactory propertyNodeFactory, ICodeGenMethodNodeFactory methodNodeFactory, ICodeGenOperatorNodeFactory operatorNodeFactory,
            INameCollisionService nameCollisionService) {
            _namePoolService = namePoolService;
            _attributeNodeFactory = attributeNodeFactory;
            _constructorNodeFactory = constructorNodeFactory;
            _propertyNodeFactory = propertyNodeFactory;
            _methodNodeFactory = methodNodeFactory;
            _operatorNodeFactory = operatorNodeFactory;
            _nameCollisionService = nameCollisionService;
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
                // constructors.Add(GenerateCodeGenConstructorNode());
                classConstructors = constructors.ToArray();
            }

            var classProperties = classNode.properties;
            CodeGenPropertyNode[]? properties = null;
            if (classProperties.Length > 0) {
                var propertyNames = new HashSet<string> { className };

                properties = new CodeGenPropertyNode[classProperties.Length];
                for (var i = 0; i < classProperties.Length; i++) {
                    properties[i] = _propertyNodeFactory.GenerateCodeGenPropertyNode(classProperties[i]);
                    properties[i].name = _nameCollisionService.GetNonCollidingName(properties[i].name, propertyNames);
                }
            }

            CodeGenMethodNode[]? methods = null;
            var classMethods = classNode.functions;
            if (classMethods.Length > 0) {
                var methodNames = new HashSet<string> { className };

                methods = new CodeGenMethodNode[classMethods.Length];
                for (var i = 0; i < classMethods.Length; i++) {
                    methods[i] = _methodNodeFactory.GenerateCodeGenMethodNode(classMethods[i], i);
                    methods[i].name = _nameCollisionService.GetNonCollidingName(methods[i].name, methodNames);
                }
            }

            var modifiers = new StringBuilder(PUBLIC);
            if (properties is not null) {
                modifiers.Append($"{WHITE_SPACE}{UNSAFE}");
            }
            if (classNode.nodeClass->ClassFlags.HasFlag(EClassFlags.CLASS_Abstract)) {
                modifiers.Append($"{WHITE_SPACE}{ABSTRACT}");
            }

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
                propertyNodes = properties,
                methodNodes = methods,
                modifier = modifiers.ToString(),
                name = className,
                attributes = attributes.ToArray(),
                baseType = baseClassName,
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

            var modifiers = PUBLIC;

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