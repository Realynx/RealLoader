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
            var modifiers = "protected";

            var attributes = new[] {
                _attributeNodeFactory.GenerateAttribute(COMPILER_GENERATED_ATTRIBUTE),
            };

            var arguments = new[] {
                new CodeGenArgumentNode {
                    type = "nint",
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

            var baseConstructor = $"base({CONSTRUCTOR_ADDRESS_NAME}, {CONSTRUCTOR_UNREAL_REFLECTION_NAME}, {CONSTRUCTOR_GLOBAL_OBJECTS_TRACKER_NAME})";

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
            var modifiers = "public";

            string name = null!;

            CodeGenAttributeNode[]? attributes = null;

            CodeGenArgumentNode[]? arguments = null;

            string? baseConstructor = null;

            string[]? body = null;

            return new CodeGenConstructorNode {
                modifier = modifiers,
                name = name,
                attributes = attributes,
                arguments = arguments,
                baseConstructor = baseConstructor,
                body = body,
            };
        }
    }
}