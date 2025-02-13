using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

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

            codeBuilder.AppendLine($"#pragma warning disable {ADDRESS_OF_MANAGED_TYPE_WARNING_CODE}");
            codeBuilder.AppendLine();

            if (namespaceNode.imports is not null) {
                foreach (var import in namespaceNode.imports) {
                    codeBuilder.AppendLine($"using {import};");
                }

                codeBuilder.AppendLine();
            }

            var fullyQualifiedNamespace = namespaceNode.fullNamespace;
            codeBuilder.AppendLine($$"""namespace {{fullyQualifiedNamespace}} {""");

            foreach (var classNode in namespaceNode.classes) {
                _classGenerator.GenerateClass(codeBuilder, classNode);
                codeBuilder.AppendLine();
            }

            // Remove trailing newline between classes end and namespace closing bracket
            codeBuilder.RemoveNewLine();

            codeBuilder.Append('}');
        }
    }
}