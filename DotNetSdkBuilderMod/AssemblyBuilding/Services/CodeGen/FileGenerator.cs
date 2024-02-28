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

            codeBuilder.Append(POUND);
            codeBuilder.Append(PRAGMA);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(WARNING);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(DISABLE);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(ADDRESS_OF_MANAGED_TYPE_WARNING_CODE);
            codeBuilder.AppendLine();

            if (namespaceNode.imports is not null) {
                foreach (var import in namespaceNode.imports) {
                    codeBuilder.Append(USING);
                    codeBuilder.Append(WHITE_SPACE);
                    codeBuilder.Append(import);
                    codeBuilder.AppendLine(SEMICOLON);
                }

                codeBuilder.AppendLine();
            }

            codeBuilder.Append(NAMESPACE);
            codeBuilder.Append(WHITE_SPACE);

            var fullyQualifiedNamespace = namespaceNode.fullNamespace;
            codeBuilder.Append(fullyQualifiedNamespace);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

            foreach (var classNode in namespaceNode.classes) {
                _classGenerator.GenerateClass(codeBuilder, classNode);
                codeBuilder.AppendLine();
            }

            // Remove trailing newline between classes end and namespace closing bracket
            codeBuilder.RemoveNewLine();

            codeBuilder.Append(CLOSED_CURLY_BRACKET);
        }
    }
}