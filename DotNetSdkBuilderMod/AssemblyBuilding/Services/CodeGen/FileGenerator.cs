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
        private readonly IPackageNameGenerator _nameSpaceGenerator;

        public FileGenerator(ILogger logger, IClassGenerator classGenerator, IPackageNameGenerator nameSpaceGenerator) {
            _logger = logger;
            _classGenerator = classGenerator;
            _nameSpaceGenerator = nameSpaceGenerator;
        }

        public unsafe void GenerateFile(StringBuilder codeBuilder, CodeGenNamespaceNode namespaceNode) {
            codeBuilder.AppendLine(namespaceNode.imports);

            codeBuilder.AppendLine();

            codeBuilder.Append(NAMESPACE);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(namespaceNode.nameSpace);

            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

            foreach (var classNode in namespaceNode.classes)
            {
                _classGenerator.GenerateClass(codeBuilder, classNode);
                codeBuilder.AppendLine();
            }

            if (namespaceNode.classes.Length > 0) {
                // Remove trailing newline between classes end and namespace closing bracket
                codeBuilder.RemoveLine();
            }

            codeBuilder.Append(CLOSED_CURLY_BRACKET);
        }
    }
}