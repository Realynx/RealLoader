using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

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
            var imports = GetImports(classNode, nameSpace);
            foreach (var import in imports) {
                codeBuilder.Append(USING);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(import);
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

        private string[] GetImports(ClassNode classNode, string currentNamespace) {
            var imports = new HashSet<string>();

            foreach (var propertyImport in GetPropertyImports(classNode.properties)) {
                if (propertyImport.StartsWith(currentNamespace)) {
                    continue;
                }

                imports.Add(propertyImport);
            }

            foreach (var propertyImport in GetFunctionImports(classNode.functions)) {
                if (propertyImport.StartsWith(currentNamespace)) {
                    continue;
                }

                imports.Add(propertyImport);
            }

            return imports.ToArray();
        }

        private IEnumerable<string> GetPropertyImports(FProperty[] properties) {
            return Enumerable.Empty<string>();
        }

        private IEnumerable<string> GetFunctionImports(UFunction[] functions) {
            return Enumerable.Empty<string>();
        }
    }
}
