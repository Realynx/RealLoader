using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenConstructorNodeFactory : ICodeGenConstructorNodeFactory {
        private readonly ILogger _logger;

        public CodeGenConstructorNodeFactory(ILogger logger) {
            _logger = logger;
        }

        public unsafe CodeGenConstructorNode GenerateDefaultConstructor(ClassNode classNode, string className) {
            CodeGenAttributeNode[]? attributes = null;

            var arguments = new[] {
                new CodeGenArgumentNode {
                    type = INT_PTR,
                    name = CONSTRUCTOR_ADDRESS_NAME,
                    attributes = null,
                }
            };

            var baseConstructor = $"{BASE}{OPEN_ROUND_BRACKET}{CONSTRUCTOR_ADDRESS_NAME}{CLOSED_ROUND_BRACKET}";

            string[]? body = null;

            return new CodeGenConstructorNode {
                modifier = PUBLIC,
                name = className,
                attributes = attributes,
                arguments = arguments,
                baseConstructor = baseConstructor,
                body = body,
            };
        }

        public unsafe CodeGenConstructorNode GenerateCodeGenConstructorNode() {
            // TODO
            string name = null!;

            CodeGenAttributeNode[]? attributes = null;

            CodeGenArgumentNode[]? arguments = null;

            string? baseConstructor = null;

            string[]? body = null;

            return new CodeGenConstructorNode
            {
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