using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ClassGenerator : IClassGenerator {
        private readonly ILogger _logger;
        private readonly IConstructorGenerator _constructorGenerator;
        private readonly IPropertyGenerator _propertyGenerator;
        private readonly IMethodGenerator _methodGenerator;
        private readonly IOperatorGenerator _operatorGenerator;

        public ClassGenerator(ILogger logger, IConstructorGenerator constructorGenerator, IPropertyGenerator propertyGenerator,
            IMethodGenerator methodGenerator, IOperatorGenerator operatorGenerator) {
            _logger = logger;
            _constructorGenerator = constructorGenerator;
            _propertyGenerator = propertyGenerator;
            _methodGenerator = methodGenerator;
            _operatorGenerator = operatorGenerator;
        }

        public unsafe void GenerateClass(StringBuilder codeBuilder, CodeGenClassNode classNode) {
            if (classNode.attributes != null) {
                foreach (var attribute in classNode.attributes) {
                    codeBuilder.AppendIndentedLine(attribute, 1);
                }
            }

            codeBuilder.AppendIndented(classNode.modifier, 1);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(CLASS);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(classNode.name);
            codeBuilder.Append(WHITE_SPACE);

            var baseClass = classNode.baseType;
            if (baseClass is not null) {
                codeBuilder.Append(COLON);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.Append(baseClass);
                codeBuilder.Append(WHITE_SPACE);
            }

            if (classNode.constructorNodes is null && classNode.propertyNodes is null && classNode.methodNodes is null && classNode.operatorNodes is null) {
                codeBuilder.Append(OPEN_CURLY_BRACKET);
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(CLOSED_CURLY_BRACKET);
                return;
            }

            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);
            if (classNode.constructorNodes is not null) {
                foreach (var constructor in classNode.constructorNodes) {
                    _constructorGenerator.GenerateConstructor(codeBuilder, constructor);
                    codeBuilder.AppendLine();
                }
            }

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

            if (classNode.operatorNodes is not null) {
                foreach (var classOperator in classNode.operatorNodes) {
                    _operatorGenerator.GenerateOperator(codeBuilder, classOperator);
                    codeBuilder.AppendLine();
                }
            }

            if (classNode.constructorNodes is not null || classNode.propertyNodes is not null || classNode.methodNodes is not null || classNode.operatorNodes is not null) {
                // Remove trailing newline between members end and class closing bracket
                codeBuilder.RemoveLine();
            }

            codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 1);
        }
    }
}
