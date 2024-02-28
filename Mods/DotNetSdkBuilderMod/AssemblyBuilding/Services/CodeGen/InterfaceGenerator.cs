using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

using static DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen.CodeGenConstants;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.CodeGen {
    public class InterfaceGenerator : IInterfaceGenerator {
        private readonly ILogger _logger;
        private readonly IGenericGenerator _genericGenerator;

        public InterfaceGenerator(ILogger logger, IGenericGenerator genericGenerator) {
            _logger = logger;
            _genericGenerator = genericGenerator;
        }

        public void GenerateInterfaces(StringBuilder codeBuilder, CodeGenInterfaceNode[] interfaceNodes) {
            if (interfaceNodes.Length == 0) {
                return;
            }

            foreach (var interfaceNode in interfaceNodes) {
                codeBuilder.Append(interfaceNode.name);

                if (interfaceNode.genericTypes is not null) {
                    _genericGenerator.GenerateGenerics(codeBuilder, interfaceNode.genericTypes);
                }

                codeBuilder.Append(COMMA);
                codeBuilder.Append(WHITE_SPACE);
            }

            // Remove the trailing comma and whitespace
            var separatorWidth = COMMA.Length + WHITE_SPACE.Length;
            codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);
        }
    }
}