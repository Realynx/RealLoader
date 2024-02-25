using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class MethodGenerator : IMethodGenerator {
        private readonly ILogger _logger;
        private readonly IAttributeGenerator _attributeGenerator;
        private readonly IArgumentGenerator _argumentGenerator;

        public MethodGenerator(ILogger logger, IAttributeGenerator attributeGenerator, IArgumentGenerator argumentGenerator) {
            _logger = logger;
            _attributeGenerator = attributeGenerator;
            _argumentGenerator = argumentGenerator;
        }

        public unsafe void GenerateMethod(StringBuilder codeBuilder, CodeGenMethodNode methodNode) {
            if (methodNode.attributes is not null) {
                foreach (var attribute in methodNode.attributes) {
                    _attributeGenerator.GenerateAttribute(codeBuilder, attribute, 2);
                }
            }

            codeBuilder.AppendIndented(methodNode.modifier, 2);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(methodNode.returnType);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(methodNode.name);

            if (methodNode.genericTypes is not null) {
                codeBuilder.Append(OPEN_ANGLE_BRACKET);

                foreach (var generic in methodNode.genericTypes) {
                    codeBuilder.Append(generic);
                    codeBuilder.Append($"{COMMA}{WHITE_SPACE}");
                }

                // Remove the trailing comma and whitespace
                var separatorWidth = COMMA.Length + WHITE_SPACE.Length;
                codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);

                codeBuilder.Append(CLOSED_ANGLE_BRACKET);
            }

            codeBuilder.Append(OPEN_ROUND_BRACKET);

            if (methodNode.arguments is not null) {
                _argumentGenerator.GenerateArguments(codeBuilder, methodNode.arguments);
            }

            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);

            if (methodNode.body is not null) {
                codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

                foreach (var str in methodNode.body) {
                    if (string.IsNullOrWhiteSpace(str)) {
                        codeBuilder.AppendLine();
                    }
                    else {
                        codeBuilder.AppendIndentedLine(str, 3);
                    }
                }

                codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 2);
            }
            else {
                codeBuilder.Append(OPEN_CURLY_BRACKET);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
            }
        }
    }
}