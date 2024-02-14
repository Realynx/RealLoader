using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class FileGenerator : IFileGenerator {
        private readonly ILogger _logger;
        private readonly IClassGenerator _classGenerator;

        public FileGenerator(ILogger logger, IClassGenerator classGenerator) {
            _logger = logger;
            _classGenerator = classGenerator;
        }

        public unsafe void GenerateFile(StringBuilder codeBuilder, ClassNode classNode, string nameSpace) {
            // Usings
            // TODO: Figure out usings
            for (var i = 0; i < 1; i++) {
                codeBuilder.Append(USING);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append("TODO: types");
                codeBuilder.AppendLine(SEMICOLON);
            }

            codeBuilder.AppendLine();
            codeBuilder.AppendLine();

            // Namespace
            codeBuilder.Append(NAMESPACE);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(nameSpace);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

            _classGenerator.GenerateClass(codeBuilder, classNode);

            codeBuilder.Append(CLOSED_CURLY_BRACKET);
        }
    }
}
