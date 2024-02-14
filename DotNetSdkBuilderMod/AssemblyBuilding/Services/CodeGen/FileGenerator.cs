using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.UClassStructs;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class FileGenerator : IFileGenerator {
        private readonly ILogger _logger;
        private readonly IClassGenerator _classGenerator;
        private readonly INameSpaceGenerator _nameSpaceGenerator;
        private readonly UnrealReflection _unrealReflection;

        public FileGenerator(ILogger logger, IClassGenerator classGenerator, INameSpaceGenerator nameSpaceGenerator, UnrealReflection unrealReflection) {
            _logger = logger;
            _classGenerator = classGenerator;
            _nameSpaceGenerator = nameSpaceGenerator;
            _unrealReflection = unrealReflection;
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

            foreach (var functionImport in GetFunctionImports(classNode.functions)) {
                if (functionImport.StartsWith(currentNamespace)) {
                    continue;
                }

                imports.Add(functionImport);
            }

            return imports.ToArray();
        }

        private unsafe string[] GetPropertyImports(FProperty[] properties) {
            var namespaces = new List<string>();

            foreach (var property in properties) {
                var nameSpace = _nameSpaceGenerator.GetNameSpace(*(UObjectBase*)property.baseFField.classPrivate);
                namespaces.Add(nameSpace);
            }

            return namespaces.ToArray();
        }

        private unsafe string[] GetFunctionImports(UFunction[] functions) {
            var namespaces = new List<string>();

            foreach (var function in functions) {
                var signature = _unrealReflection.GetFunctionSignature(function);
                if (signature.returnValue.HasValue) {
                    var nameSpace = _nameSpaceGenerator.GetNameSpace(*(UObjectBase*)signature.returnValue.Value.classPrivate);
                    namespaces.Add(nameSpace);
                }

                foreach (var parameter in signature.parameters) {
                    var nameSpace = _nameSpaceGenerator.GetNameSpace(*(UObjectBase*)parameter.classPrivate);
                    namespaces.Add(nameSpace);
                }
            }

            return namespaces.ToArray();
        }
    }
}