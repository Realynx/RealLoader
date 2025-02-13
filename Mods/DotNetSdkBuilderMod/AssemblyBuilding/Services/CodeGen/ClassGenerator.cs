using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ClassGenerator : IClassGenerator {
        private readonly ILogger _logger;
        private readonly IAttributeGenerator _attributeGenerator;
        private readonly IConstructorGenerator _constructorGenerator;
        private readonly IPropertyGenerator _propertyGenerator;
        private readonly IMethodGenerator _methodGenerator;
        private readonly IOperatorGenerator _operatorGenerator;
        private readonly IInterfaceGenerator _interfaceGenerator;

        public ClassGenerator(ILogger logger, IAttributeGenerator attributeGenerator, IConstructorGenerator constructorGenerator,
            IPropertyGenerator propertyGenerator, IMethodGenerator methodGenerator, IOperatorGenerator operatorGenerator,
            IInterfaceGenerator interfaceGenerator) {
            _logger = logger;
            _attributeGenerator = attributeGenerator;
            _constructorGenerator = constructorGenerator;
            _propertyGenerator = propertyGenerator;
            _methodGenerator = methodGenerator;
            _operatorGenerator = operatorGenerator;
            _interfaceGenerator = interfaceGenerator;
        }

        public unsafe void GenerateClass(StringBuilder codeBuilder, CodeGenClassNode classNode) {
            if (classNode.attributes is not null) {
                foreach (var attribute in classNode.attributes) {
                    _attributeGenerator.GenerateAttribute(codeBuilder, attribute, 1);
                    codeBuilder.AppendLine();
                }
            }

            codeBuilder.AppendIndented(classNode.modifier, 1);
            codeBuilder.Append($" class {classNode.name}");

            var baseClass = classNode.baseType;
            if (baseClass is not null) {
                codeBuilder.Append($" : {baseClass}");
            }

            if (classNode.interfaces is { Length: > 0 }) {
                if (baseClass is not null) {
                    codeBuilder.Append(',');
                }

                codeBuilder.Append(' ');
                _interfaceGenerator.GenerateInterfaces(codeBuilder, classNode.interfaces);
            }

            codeBuilder.Append(' ');

            if (classNode.constructorNodes is null && classNode.propertyNodes is null && classNode.methodNodes is null && classNode.operatorNodes is null) {
                codeBuilder.AppendLine("{ }");
                return;
            }

            codeBuilder.AppendLine("{");
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

            if (classNode.interfaces is not null) {
                foreach (var interfaceNode in classNode.interfaces) {
                    foreach (var method in interfaceNode.methodNodes) {
                        _methodGenerator.GenerateMethod(codeBuilder, method);
                        codeBuilder.AppendLine();
                    }
                }
            }

            if (classNode.operatorNodes is not null) {
                foreach (var classOperator in classNode.operatorNodes) {
                    _operatorGenerator.GenerateOperator(codeBuilder, classOperator);
                    codeBuilder.AppendLine();
                }
            }

            if (classNode.constructorNodes is not null || classNode.propertyNodes is not null || classNode.methodNodes is not null || classNode.operatorNodes is not null || classNode.interfaces is not null) {
                // Remove trailing newline between members end and class closing bracket
                codeBuilder.RemoveNewLine();
            }

            codeBuilder.AppendIndentedLine("}", 1);
        }
    }
}