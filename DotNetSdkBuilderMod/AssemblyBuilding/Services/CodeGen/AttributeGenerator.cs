using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class AttributeGenerator : IAttributeGenerator {
        private readonly ILogger _logger;

        public AttributeGenerator(ILogger logger) {
            _logger = logger;
        }

        public unsafe void GenerateAttribute(StringBuilder codeBuilder, CodeGenAttributeNode attributeNode, int indent) {
            codeBuilder.AppendIndented(OPEN_SQUARE_BRACKET, indent);
            codeBuilder.Append(attributeNode.name);

            if (attributeNode.value is not null) {
                codeBuilder.Append(OPEN_ROUND_BRACKET);
                codeBuilder.Append(attributeNode.value);
                codeBuilder.Append(CLOSED_ROUND_BRACKET);
            }

            codeBuilder.AppendLine(CLOSED_SQUARE_BRACKET);
        }
    }
}