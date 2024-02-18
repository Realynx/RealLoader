using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ConstructorGenerator : IConstructorGenerator {
        private readonly ILogger _logger;
        private readonly IArgumentGenerator _argumentGenerator;

        public ConstructorGenerator(ILogger logger, IArgumentGenerator argumentGenerator) {
            _logger = logger;
            _argumentGenerator = argumentGenerator;
        }

        public void GenerateConstructor(StringBuilder codeBuilder, CodeGenConstructorNode constructorNode) {
            codeBuilder.AppendIndented(constructorNode.modifier, 2);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(constructorNode.name);
            codeBuilder.Append(OPEN_ROUND_BRACKET);

            if (constructorNode.arguments is not null) {
                _argumentGenerator.GenerateArguments(codeBuilder, constructorNode.arguments);
            }

            codeBuilder.Append(CLOSED_ROUND_BRACKET);
            codeBuilder.Append(WHITE_SPACE);

            if (constructorNode.baseConstructor is not null) {
                codeBuilder.Append(COLON);
                codeBuilder.Append(WHITE_SPACE);

                codeBuilder.Append(constructorNode.baseConstructor);
                codeBuilder.Append(WHITE_SPACE);
            }

            if (constructorNode.body is null) {
                codeBuilder.Append(OPEN_CURLY_BRACKET);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
            }
            else {
                codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

                foreach (var line in constructorNode.body) {
                    codeBuilder.AppendIndentedLine(line, 3);
                }

                codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 2);
            }
        }
    }
}