using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class InMemoryCompiler {
        private readonly ILogger _logger;

        public InMemoryCompiler(ILogger logger) {
            _logger = logger;
        }
    }
}
