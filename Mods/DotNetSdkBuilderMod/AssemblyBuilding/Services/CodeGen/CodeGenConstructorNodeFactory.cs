using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.EngineServices.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenConstructorNodeFactory : ICodeGenConstructorNodeFactory {
        private readonly ILogger _logger;
        private readonly ICodeGenAttributeNodeFactory _attributeNodeFactory;

        public CodeGenConstructorNodeFactory(ILogger logger, ICodeGenAttributeNodeFactory attributeNodeFactory) {
            _logger = logger;
            _attributeNodeFactory = attributeNodeFactory;
        }

        public CodeGenConstructorNode GenerateDefaultConstructor(string className) {
            var modifiers = PROTECTED;

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE)
            };

            var arguments = new[] {
                new CodeGenArgumentNode {
                    type = INT_PTR,
                    name = CONSTRUCTOR_ADDRESS_NAME,
                },
                new CodeGenArgumentNode {
                    type = nameof(IUnrealReflection),
                    name = CONSTRUCTOR_UNREAL_REFLECTION_NAME,
                },
                new CodeGenArgumentNode {
                    type = nameof(IGlobalObjectsTracker),
                    name = CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME,
                }
            };

            var baseConstructor = $"{BASE}{OPEN_ROUND_BRACKET}{CONSTRUCTOR_ADDRESS_NAME}{COMMA}{WHITE_SPACE}{CONSTRUCTOR_UNREAL_REFLECTION_NAME}{COMMA}{WHITE_SPACE}{CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME}{CLOSED_ROUND_BRACKET}";

            return new CodeGenConstructorNode {
                modifier = modifiers,
                name = className,
                attributes = attributes,
                arguments = arguments,
                baseConstructor = baseConstructor,
            };
        }

        public unsafe CodeGenConstructorNode GenerateCodeGenConstructorNode() {
            // TODO
            string name = null!;

            CodeGenAttributeNode[]? attributes = null;

            CodeGenArgumentNode[]? arguments = null;

            string? baseConstructor = null;

            string[]? body = null;

            return new CodeGenConstructorNode {
                modifier = PUBLIC,
                name = name,
                attributes = attributes,
                arguments = arguments,
                baseConstructor = baseConstructor,
                body = body,
            };
        }
    }
}