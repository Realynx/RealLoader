using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;
using DotNetSdkBuilderMod.Extensions;

using PalworldManagedModFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class PropertyGenerator : IPropertyGenerator {
        private readonly ILogger _logger;

        public PropertyGenerator(ILogger logger) {
            _logger = logger;
        }

        public unsafe void GenerateProperty(StringBuilder codeBuilder, CodeGenPropertyNode propertyNode) {
            codeBuilder.AppendIndented(PRIVATE, 2);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(INT_PTR);
            codeBuilder.Append(WHITE_SPACE);
            codeBuilder.Append(propertyNode.runtimeAddressFieldName);
            codeBuilder.AppendLine(SEMICOLON);

            codeBuilder.AppendIndented(propertyNode.modifer, 2);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(propertyNode.returnType);
            codeBuilder.Append(WHITE_SPACE);

            codeBuilder.Append(propertyNode.name);

            var hasGetter = !string.IsNullOrWhiteSpace(propertyNode.get);
            var hasSetter = !string.IsNullOrWhiteSpace(propertyNode.set);

            if (!hasGetter && !hasSetter) {
                codeBuilder.AppendLine(COLON);
            }
            else {
                codeBuilder.Append(WHITE_SPACE);
                codeBuilder.AppendLine(OPEN_CURLY_BRACKET);

                if (hasGetter) {
                    codeBuilder.AppendIndentedLine(propertyNode.get, 3);
                }

                if (hasSetter) {
                    codeBuilder.AppendIndentedLine(propertyNode.set, 3);
                }

                codeBuilder.AppendIndentedLine(CLOSED_CURLY_BRACKET, 2);
            }
        }
    }
}
