using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.Flags;
using PalworldManagedModFramework.Sdk.Models.CoreUObject.UClassStructs;
using PalworldManagedModFramework.Sdk.Services.Interfaces;

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
            var propertyName = _namePoolService.GetNameString(property->ObjectName);

            CodeGenAttributeNode[]? attributes = null;
            if (property->propertyFlags.HasFlag(EPropertyFlags.CPF_Deprecated)) {
                attributes = new[] {
                    _attributeNodeFactory.GenerateAttribute(DEPRECATED_ATTRIBUTE)
                };
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
    }
}