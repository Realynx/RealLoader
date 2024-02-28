using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ArgumentGenerator : IArgumentGenerator {
        private readonly ILogger _logger;
        private readonly IAttributeGenerator _attributeGenerator;

        public ArgumentGenerator(ILogger logger, IAttributeGenerator attributeGenerator) {
            _logger = logger;
            _attributeGenerator = attributeGenerator;
        }

        public void GenerateArguments(StringBuilder codeBuilder, CodeGenArgumentNode[] argumentNodes) {
            if (argumentNodes.Length == 0) {
                return;
            }

            foreach (var argumentNode in argumentNodes) {
                if (argumentNode.attributes is not null) {
                    foreach (var attributeNode in argumentNode.attributes) {
                        _attributeGenerator.GenerateAttribute(codeBuilder, attributeNode, 0);

                        // GenerateAttribute ends with a new line. We need to remove that.
                        codeBuilder.RemoveNewLine();
                        codeBuilder.Append(WHITE_SPACE);
                    }
                }

                if (argumentNode.modifier is not null) {
                    codeBuilder.Append(argumentNode.modifier);
                    codeBuilder.Append(WHITE_SPACE);
                }

                codeBuilder.Append(argumentNode.type);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(argumentNode.name);
                codeBuilder.Append(COMMA);
                codeBuilder.Append(WHITE_SPACE);
            }

            // Remove the trailing comma and whitespace
            var separatorWidth = COMMA.Length + WHITE_SPACE.Length;
            codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);
        }
    }
}