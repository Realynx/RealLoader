using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

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

            codeBuilder.Append('<');

            const string GENERIC_SEPARATOR = ", ";
            foreach (var generic in genericTypes) {
                codeBuilder.Append(generic);
                codeBuilder.Append(GENERIC_SEPARATOR);
            }

            // Remove the trailing comma and whitespace
            var separatorWidth = GENERIC_SEPARATOR.Length;
            codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);

            codeBuilder.Append('>');
        }
    }
}