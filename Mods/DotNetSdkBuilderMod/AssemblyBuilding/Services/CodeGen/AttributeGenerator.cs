using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class AttributeGenerator : IAttributeGenerator {
        private readonly ILogger _logger;

        public AttributeGenerator(ILogger logger) {
            _logger = logger;
        }

        public unsafe void GenerateAttribute(StringBuilder codeBuilder, CodeGenAttributeNode attributeNode, int indent) {
            codeBuilder.AppendIndent(indent);
            codeBuilder.Append($"[{attributeNode.name}");

            if (attributeNode.value is not null) {
                codeBuilder.Append($"({attributeNode.value})");
            }

            codeBuilder.Append(']');
        }
    }
}