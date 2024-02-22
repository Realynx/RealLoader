using System.Text;

using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class InMemoryCompiler : ICodeCompiler {
        private readonly ILogger _logger;
        private readonly string _buildLocation;

        public InMemoryCompiler(ILogger logger, string buildLocation) {
            _logger = logger;
            _buildLocation = buildLocation;
        }

        public void AppendFile(StringBuilder code, string nameSpace) {
            throw new NotImplementedException();
        }

        public void Compile() {
            throw new NotImplementedException();
        }
    }
}
