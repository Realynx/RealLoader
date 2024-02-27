using DotNetSdkBuilderMod.AssemblyBuilding.Models;
using DotNetSdkBuilderMod.AssemblyBuilding.Services.Interfaces;

using PalworldManagedModFramework.Sdk.Logging;

namespace DotNetSdkBuilderMod.AssemblyBuilding.Services.Compile {
    public class CodeCompilerFactory : ICodeCompilerFactory {
        private readonly ILogger _logger;
        private readonly SdkBuilderConfig _compilerConfig;

        public CodeCompilerFactory(ILogger logger, SdkBuilderConfig compilerConfig) {
            _logger = logger;
            _compilerConfig = compilerConfig;
        }

        public ICodeCompiler CreateCompiler() {
            var buildLocation = Path.GetFullPath(_compilerConfig.BuildLocation);

            if (_compilerConfig.BuildInMemory) {
                return new InMemoryCompiler(_logger, buildLocation);
            }

            return new OnDiskCompiler(_logger, buildLocation, _compilerConfig.DisplayCompilerOutput);
        }
    }
}