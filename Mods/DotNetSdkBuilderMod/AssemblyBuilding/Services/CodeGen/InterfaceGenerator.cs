using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using RealLoaderFramework.Sdk.Logging;

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

            const string INTERFACE_SEPARATOR = ", ";
            foreach (var interfaceNode in interfaceNodes) {
                codeBuilder.Append(interfaceNode.name);

                if (interfaceNode.genericTypes is not null) {
                    _genericGenerator.GenerateGenerics(codeBuilder, interfaceNode.genericTypes);
                }

                codeBuilder.Append(INTERFACE_SEPARATOR);
            }

            // Remove the trailing comma and whitespace
            var separatorWidth = INTERFACE_SEPARATOR.Length;
            codeBuilder.Remove(codeBuilder.Length - separatorWidth, separatorWidth);
        }
    }
}