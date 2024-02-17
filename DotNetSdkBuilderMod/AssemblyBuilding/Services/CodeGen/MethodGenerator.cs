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

        public MethodGenerator(ILogger logger, IAttributeGenerator attributeGenerator) {
            _logger = logger;
            _attributeGenerator = attributeGenerator;
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

            codeBuilder.Append(OPEN_ROUND_BRACKET);

            if (methodNode.arguments is not null) {
                var joinedArgs = string.Join($"{COMMA}{WHITE_SPACE}", methodNode.arguments.Select(x => $"{x.type}{WHITE_SPACE}{x.name}"));
                codeBuilder.Append(joinedArgs);
            }

            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(OPEN_CURLY_BRACKET);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
        }
    }
}