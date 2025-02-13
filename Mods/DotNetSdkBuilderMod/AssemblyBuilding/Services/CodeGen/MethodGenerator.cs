using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class MethodGenerator : IMethodGenerator {
        private readonly ILogger _logger;
        private readonly IAttributeGenerator _attributeGenerator;
        private readonly IArgumentGenerator _argumentGenerator;
        private readonly IGenericGenerator _genericGenerator;

        public MethodGenerator(ILogger logger, IAttributeGenerator attributeGenerator, IArgumentGenerator argumentGenerator,
            IGenericGenerator genericGenerator) {
            _logger = logger;
            _attributeGenerator = attributeGenerator;
            _argumentGenerator = argumentGenerator;
            _genericGenerator = genericGenerator;
        }

        public unsafe void GenerateMethod(StringBuilder codeBuilder, CodeGenMethodNode methodNode) {
            if (methodNode.attributes is not null) {
                foreach (var attribute in methodNode.attributes) {
                    _attributeGenerator.GenerateAttribute(codeBuilder, attribute, 2);
                    codeBuilder.AppendLine();
                }
            }

            codeBuilder.AppendIndent(2);
            codeBuilder.Append($"{methodNode.modifier} {methodNode.returnType} {methodNode.name}");

            if (methodNode.genericTypes is not null) {
                _genericGenerator.GenerateGenerics(codeBuilder, methodNode.genericTypes);
            }

            codeBuilder.Append('(');

            if (methodNode.arguments is not null) {
                _argumentGenerator.GenerateArguments(codeBuilder, methodNode.arguments);
            }

            codeBuilder.Append(") ");

            if (methodNode.body is not null) {
                codeBuilder.AppendLine("{");

                foreach (var str in methodNode.body) {
                    if (string.IsNullOrWhiteSpace(str)) {
                        codeBuilder.AppendLine();
                    }
                    else {
                        codeBuilder.AppendIndentedLine(str, 3);
                    }
                }

                codeBuilder.AppendIndentedLine("}", 2);
            }
            else {
                codeBuilder.AppendLine("{ }");
            }
        }
    }
}