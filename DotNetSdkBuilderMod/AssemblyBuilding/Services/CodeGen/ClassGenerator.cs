using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.UnrealSdk.Services.Interfaces;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class ClassGenerator : IClassGenerator {
        private readonly ILogger _logger;
        private readonly IGlobalObjects _globalObjects;
        private readonly IPropertiesGenerator _propertiesGenerator;
        private readonly IMethodGenerator _methodGenerator;

        public ClassGenerator(ILogger logger, IGlobalObjects globalObjects, IPropertiesGenerator propertiesGenerator, IMethodGenerator methodGenerator) {
            _logger = logger;
            _globalObjects = globalObjects;
            _propertiesGenerator = propertiesGenerator;
            _methodGenerator = methodGenerator;
        }

        public void GenerateClass(StringBuilder codeBuilder, ClassNode classNode) {
            var modifers = GetClassModifers(classNode);

            codeBuilder.AppendIndented(modifers, 1);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(CLASS);
            codeBuilder.Append(WHITE_SPACE);

            var className = _globalObjects.GetNameString(classNode.ClassName);

            codeBuilder.Append(className);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

            var properties = classNode.properties;
            foreach (var property in properties) {
                _propertiesGenerator.GenerateProperty(codeBuilder, property);
                codeBuilder.AppendLine();
            }

            codeBuilder.AppendLine();

            var methods = classNode.functions;
            foreach (var method in methods) {
                _methodGenerator.GenerateMethod(codeBuilder, method);
                codeBuilder.AppendLine();
            }

            codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 1);
        }

        private string GetClassModifers(ClassNode classNode) {
            // TODO: At some point we may want to get more details here.
            return PUBLIC;
        }
    }
}
