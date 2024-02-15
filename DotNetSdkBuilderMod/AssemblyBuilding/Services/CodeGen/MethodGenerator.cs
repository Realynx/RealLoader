using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class MethodGenerator : IMethodGenerator {
        private readonly ILogger _logger;

        public MethodGenerator(ILogger logger) {
            _logger = logger;
        }

        public unsafe void GenerateMethod(StringBuilder codeBuilder, CodeGenMethodNode methodNode) {
            codeBuilder.AppendIndented(methodNode.modifer, 2);
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