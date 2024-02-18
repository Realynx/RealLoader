using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ArgumentGenerator : IArgumentGenerator {
        private readonly ILogger _logger;

        public ArgumentGenerator(ILogger logger) {
            _logger = logger;
        }

        public void GenerateArguments(StringBuilder codeBuilder, CodeGenArgumentNode[] argumentNodes) {
            if (argumentNodes.Length == 0) {
                return;
            }

            foreach (var argumentNode in argumentNodes) {
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