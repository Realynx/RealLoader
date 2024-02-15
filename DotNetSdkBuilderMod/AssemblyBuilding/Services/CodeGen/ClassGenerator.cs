using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ClassGenerator : IClassGenerator {
        private readonly ILogger _logger;
        private readonly IPropertyGenerator _propertyGenerator;
        private readonly IMethodGenerator _methodGenerator;

        public ClassGenerator(ILogger logger, IPropertyGenerator propertyGenerator, IMethodGenerator methodGenerator) {
            _logger = logger;
            _propertyGenerator = propertyGenerator;
            _methodGenerator = methodGenerator;
        }

        public unsafe void GenerateClass(StringBuilder codeBuilder, CodeGenClassNode classNode) {
            codeBuilder.AppendIndented(classNode.modifer, 1);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(CLASS);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(classNode.name);
            codeBuilder.Append(WHITE_SPACE);

            var baseClass = classNode.baseType;
            if (!string.IsNullOrWhiteSpace(baseClass)) {
                codeBuilder.Append(COLON);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(baseClass);
                codeBuilder.Append(WHITE_SPACE);
            }

            if (classNode.propertyNodes is null && classNode.methodNodes is null) {
                codeBuilder.Append(OPEN_CURLY_BRACKET);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
                return;
            }

            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

            if (classNode.propertyNodes is not null) {
                foreach (var property in classNode.propertyNodes) {
                    _propertyGenerator.GenerateProperty(codeBuilder, property);
                    codeBuilder.AppendLine();
                }
            }

            if (classNode.methodNodes is not null) {
                foreach (var method in classNode.methodNodes) {
                    _methodGenerator.GenerateMethod(codeBuilder, method);
                    codeBuilder.AppendLine();
                }
            }

            if (classNode.propertyNodes is not null || classNode.methodNodes is not null) {
                // Remove trailing newline between members end and class closing bracket
                codeBuilder.RemoveLine();
            }

            codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 1);
        }
    }
}
