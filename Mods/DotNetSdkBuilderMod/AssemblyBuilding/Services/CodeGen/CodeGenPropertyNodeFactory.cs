using System.Runtime.CompilerServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Models.CoreUObject.Flags;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenPropertyNodeFactory : ICodeGenPropertyNodeFactory {
        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;
        private readonly INameCollisionService _nameCollisionService;

        public CodeGenPropertyNodeFactory(ILogger logger, INamePoolService namePoolService, ICodeGenAttributeNodeFactory attributeNodeFactory,
            INameCollisionService nameCollisionService) {
            _logger = logger;
            _namePoolService = namePoolService;
            _attributeNodeFactory = attributeNodeFactory;
            _nameCollisionService = nameCollisionService;
        }

        public unsafe CodeGenPropertyNode GenerateOwnedPropertyNode(PropertyNode propertyNode) {
            var nonSanitizedPropertyName = _namePoolService.GetNameString(propertyNode.PropertyName);
            var propertyName = _namePoolService.GetSanitizedNameString(propertyNode.PropertyName);
            propertyName = _nameCollisionService.GetNonCollidingName(propertyName);

            var modifiers = $"{PUBLIC}{WHITE_SPACE}{VIRTUAL}";

            var attributes = new List<CodeGenAttributeNode> {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"{QUOTE}{nonSanitizedPropertyName}{QUOTE}")
            };
            if (propertyNode.nodeProperty->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes.Add(_attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var returnType = _namePoolService.GetSanitizedNameString(propertyNode.nodeProperty->baseFField.classPrivate->ObjectName);

            var fieldOffset = propertyNode.nodeProperty->offset_Internal;

            var getter = $"{GET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{STAR}{OPEN_ROUND_BRACKET}{returnType}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{SEMICOLON}";
            var setter = $"{SET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{nameof(Unsafe)}{DOT}{nameof(Unsafe.Write)}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{COMMA}{WHITE_SPACE}{OPEN_ROUND_BRACKET}{INT_PTR}{CLOSED_ROUND_BRACKET}{VALUE}{DOT}{ADDRESS_FIELD_NAME}{CLOSED_ROUND_BRACKET}{SEMICOLON}";

            return new CodeGenPropertyNode {
                modifier = modifiers,
                name = propertyName,
                attributes = attributes.ToArray(),
                returnType = returnType,
                get = getter,
                set = setter,
            };
        }

        public unsafe CodeGenPropertyNode GenerateInheritedPropertyNode(PropertyNode propertyNode) {
            var nonSanitizedPropertyName = _namePoolService.GetNameString(propertyNode.PropertyName);
            var propertyName = _namePoolService.GetSanitizedNameString(propertyNode.PropertyName);
            propertyName = _nameCollisionService.GetNonCollidingName(propertyName);

            var modifiers = $"{PUBLIC}{WHITE_SPACE}{OVERRIDE}";

            var attributes = new List<CodeGenAttributeNode> {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"{QUOTE}{nonSanitizedPropertyName}{QUOTE}")
            };
            if (propertyNode.nodeProperty->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes.Add(_attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var returnType = _namePoolService.GetSanitizedNameString(propertyNode.nodeProperty->baseFField.classPrivate->ObjectName);

            var fieldOffset = propertyNode.nodeProperty->offset_Internal;

            // TODO: Does this work?
            var getter = $"{GET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{BASE}{DOT}{propertyName}{SEMICOLON}";
            var setter = $"{SET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{BASE}{DOT}{propertyName}{WHITE_SPACE}{EQUALS}{WHITE_SPACE}{VALUE}{SEMICOLON}";

            return new CodeGenPropertyNode {
                modifier = modifiers,
                name = propertyName,
                attributes = attributes.ToArray(),
                returnType = returnType,
                get = getter,
                set = setter,
            };
        }
    }
}