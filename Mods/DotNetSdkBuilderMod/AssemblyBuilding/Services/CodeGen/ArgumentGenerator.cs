using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

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

            const string ARGUMENT_SEPARATOR = ", ";
            foreach (var argumentNode in argumentNodes) {
                if (argumentNode.attributes is not null) {
                    foreach (var attributeNode in argumentNode.attributes) {
                        _attributeGenerator.GenerateAttribute(codeBuilder, attributeNode, 0);
                        codeBuilder.Append(' ');
                    }
                }

                if (argumentNode.modifier is not null) {
                    codeBuilder.Append($"{argumentNode.modifier} ");
                }

                codeBuilder.Append($"{argumentNode.type} {argumentNode.name}{ARGUMENT_SEPARATOR}");
            }

            // Remove the trailing comma and whitespace
            var separatorWidth = ARGUMENT_SEPARATOR.Length;
            codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);
        }
    }
}