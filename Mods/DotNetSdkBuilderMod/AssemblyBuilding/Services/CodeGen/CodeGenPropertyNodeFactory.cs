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

            var modifiers = "public virtual";

            var attributes = new List<CodeGenAttributeNode> {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"\"{nonSanitizedPropertyName}\"")
            };
            if (propertyNode.nodeProperty->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes.Add(_attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var returnType = _namePoolService.GetSanitizedNameString(propertyNode.nodeProperty->baseFField.classPrivate->ObjectName);

            var fieldOffset = propertyNode.nodeProperty->offset_Internal;

            var getter = $"get => *({returnType}*)({ADDRESS_FIELD_NAME} + {fieldOffset});";
            var setter = $"set => {nameof(Unsafe)}.{nameof(Unsafe.Write)}({ADDRESS_FIELD_NAME} + {fieldOffset}, (nint)value.{ADDRESS_FIELD_NAME});";

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

            var modifiers = "public override";

            var attributes = new List<CodeGenAttributeNode> {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"\"{nonSanitizedPropertyName}\"")
            };
            if (propertyNode.nodeProperty->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes.Add(_attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var returnType = _namePoolService.GetSanitizedNameString(propertyNode.nodeProperty->baseFField.classPrivate->ObjectName);

            var fieldOffset = propertyNode.nodeProperty->offset_Internal;

            // TODO: Does this work?
            var getter = $"get => base.{propertyName};";
            var setter = $"set => base.{propertyName} = value;";

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