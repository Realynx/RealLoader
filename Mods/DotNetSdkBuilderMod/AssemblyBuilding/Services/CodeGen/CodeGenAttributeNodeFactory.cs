using System.Diagnostics.CodeAnalysis;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class CodeGenAttributeNodeFactory : ICodeGenAttributeNodeFactory {
        private readonly ILogger _logger;
        public CodeGenAttributeNodeFactory(ILogger logger) {
            _logger = logger;
        }

        public CodeGenAttributeNode GenerateAssemblyAttribute([ConstantExpected] string attributeName, string attributeValue) {
            return new CodeGenAttributeNode {
                name = $"assembly: {attributeName}",
                value = attributeValue,
            };
        }

        public CodeGenAttributeNode GenerateAttribute([ConstantExpected] string attributeName, string attributeValue) {
            return new CodeGenAttributeNode {
                name = attributeName,
                value = attributeValue,
            };
        }

        public CodeGenAttributeNode GenerateAttribute([ConstantExpected] string attributeName) {
            return new CodeGenAttributeNode {
                name = attributeName,
                value = null,
            };
        }
    }
}