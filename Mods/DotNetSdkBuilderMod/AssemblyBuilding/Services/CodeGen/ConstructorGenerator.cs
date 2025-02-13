using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ConstructorGenerator : IConstructorGenerator {
        private readonly ILogger _logger;
        private readonly IArgumentGenerator _argumentGenerator;
        private readonly IAttributeGenerator _attributeGenerator;

        public ConstructorGenerator(ILogger logger, IArgumentGenerator argumentGenerator, IAttributeGenerator attributeGenerator) {
            _logger = logger;
            _argumentGenerator = argumentGenerator;
            _attributeGenerator = attributeGenerator;
        }

        public void GenerateConstructor(StringBuilder codeBuilder, CodeGenConstructorNode constructorNode) {
            if (constructorNode.attributes is not null) {
                foreach (var attributeNode in constructorNode.attributes) {
                    _attributeGenerator.GenerateAttribute(codeBuilder, attributeNode, 2);
                    codeBuilder.AppendLine();
                }
            }

            codeBuilder.AppendIndented(constructorNode.modifier, 2);
            codeBuilder.Append($" {constructorNode.name}(");

            if (constructorNode.arguments is not null) {
                _argumentGenerator.GenerateArguments(codeBuilder, constructorNode.arguments);
            }

            codeBuilder.Append(") ");

            if (constructorNode.baseConstructor is not null) {
                codeBuilder.Append($": {constructorNode.baseConstructor} ");
            }

            if (constructorNode.body is null) {
                codeBuilder.AppendLine("{ }");
            }
            else {
                codeBuilder.AppendLine("{");

                foreach (var line in constructorNode.body) {
                    if (string.IsNullOrWhiteSpace(line)) {
                        codeBuilder.AppendLine();
                    }
                    else {
                        codeBuilder.AppendIndentedLine(line, 3);
                    }
                }

                codeBuilder.AppendIndentedLine("}", 2);
            }
        }
    }
}