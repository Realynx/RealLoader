using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using RealLoaderFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class PropertyGenerator : IPropertyGenerator {
        private readonly ILogger _logger;
        private readonly IAttributeGenerator _attributeGenerator;

        public PropertyGenerator(ILogger logger, IAttributeGenerator attributeGenerator) {
            _logger = logger;
            _attributeGenerator = attributeGenerator;
        }

        public unsafe void GenerateProperty(StringBuilder codeBuilder, CodeGenPropertyNode propertyNode) {
            if (propertyNode.attributes is not null) {
                foreach (var attribute in propertyNode.attributes) {
                    _attributeGenerator.GenerateAttribute(codeBuilder, attribute, 2);
                }
            }

            codeBuilder.AppendIndented(propertyNode.modifier, 2);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(propertyNode.returnType);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(propertyNode.name);

            var hasGetter = propertyNode.get is not null;
            var hasSetter = propertyNode.set is not null;

            if (!hasGetter && !hasSetter) {
                codeBuilder.AppendLine(COLON);
            }
            else {
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

                if (hasGetter) {
                    codeBuilder.AppendIndentedLine(propertyNode.get!, 3);
                }

                if (hasSetter) {
                    codeBuilder.AppendIndentedLine(propertyNode.set!, 3);
                }

                codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 2);
            }
        }
    }
}
