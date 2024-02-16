using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

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

        public unsafe void GenerateFile(StringBuilder codeBuilder, CodeGenNamespaceNode namespaceNode) {
            if (namespaceNode.classes is null) {
                return;
            }

            if (!string.IsNullOrWhiteSpace(namespaceNode.imports)) {
                codeBuilder.AppendLine(namespaceNode.imports);
            }

            codeBuilder.Append(NAMESPACE);
            codeBuilder.Append(WHITE_SPACE);

            var fullyQualifiedNamespace = namespaceNode.fullNameSpace.TrimStart('/').Replace('/', '.');
            codeBuilder.Append(fullyQualifiedNamespace);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

            foreach (var classNode in namespaceNode.classes) {
                _classGenerator.GenerateClass(codeBuilder, classNode);
                codeBuilder.AppendLine();
            }

            // Remove trailing newline between classes end and namespace closing bracket
            codeBuilder.RemoveLine();

            codeBuilder.Append(CLOSED_CURLY_BRACKET);
        }
    }
}