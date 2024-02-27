using System.Runtime.CompilerServices;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenPropertyNodeFactory : ICodeGenPropertyNodeFactory {
        private readonly ILogger _logger;
        private readonly INamePoolService _namePoolService;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;

        public CodeGenPropertyNodeFactory(ILogger logger, INamePoolService namePoolService, ICodeGenAttributeNodeFactory attributeNodeFactory) {
            _logger = logger;
            _namePoolService = namePoolService;
            _attributeNodeFactory = attributeNodeFactory;
        }

        public unsafe CodeGenPropertyNode GenerateCodeGenPropertyNode(FProperty* property) {
            var nonSanitizedPropertyName = _namePoolService.GetNameString(property->ObjectName);
            var propertyName = _namePoolService.GetSanitizedNameString(property->ObjectName);
            if (char.IsDigit(propertyName[0])) {
                propertyName = $"_{propertyName}";
            }

            var modifiers = PUBLIC;

            var attributes = new List<CodeGenAttributeNode> {
                _attributeNodeFactory.GenerateAttribute(ORIGINAL_MEMBER_NAME_ATTRIBUTE, $"{QUOTE}{nonSanitizedPropertyName}{QUOTE}")
            };
            if (property->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes.Add(_attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE));
            }

            var returnType = _namePoolService.GetSanitizedNameString(property->baseFField.classPrivate->ObjectName);

            var fieldOffset = property->offset_Internal;

            var getter = $"{GET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{STAR}{OPEN_ROUND_BRACKET}{returnType}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{SEMICOLON}";
            var setter = $"{SET}{WHITE_SPACE}{LAMBDA}{WHITE_SPACE}{nameof(Unsafe)}{DOT}{nameof(Unsafe.Write)}{OPEN_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{VOID}{STAR}{CLOSED_ROUND_BRACKET}{OPEN_ROUND_BRACKET}{ADDRESS_FIELD_NAME}{WHITE_SPACE}{PLUS}{WHITE_SPACE}{fieldOffset}{CLOSED_ROUND_BRACKET}{COMMA}{WHITE_SPACE}{OPEN_ROUND_BRACKET}{INT_PTR}{CLOSED_ROUND_BRACKET}{VALUE}{DOT}{ADDRESS_FIELD_NAME}{CLOSED_ROUND_BRACKET}{SEMICOLON}";

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