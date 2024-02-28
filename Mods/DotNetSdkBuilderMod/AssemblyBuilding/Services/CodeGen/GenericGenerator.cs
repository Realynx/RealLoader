using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class GenericGenerator : IGenericGenerator {
        private readonly ILogger _logger;

        public GenericGenerator(ILogger logger) {
            _logger = logger;
        }

        public void GenerateGenerics(StringBuilder codeBuilder, string[] genericTypes) {
            if (genericTypes.Length == 0) {
                return;
            }

            codeBuilder.Append(OPEN_ANGLE_BRACKET);

            foreach (var generic in genericTypes) {
                codeBuilder.Append(generic);
                codeBuilder.Append(COMMA);
                codeBuilder.Append(WHITE_SPACE);
            }

            // Remove the trailing comma and whitespace
            var separatorWidth = COMMA.Length + WHITE_SPACE.Length;
            codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);

            codeBuilder.Append(CLOSED_ANGLE_BRACKET);
        }
    }
}