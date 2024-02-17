using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class PatternScanner {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IPatternResolver _patternResolver;
        private readonly IUObjectFuncs _uObjectFuncs;

        public PatternScanner(ILogger logger, IEnginePattern enginePattern, IPatternResolver patternResolver, IUObjectFuncs uObjectFuncs) {
            _logger = logger;
            _enginePattern = enginePattern;
            _patternResolver = patternResolver;
            _uObjectFuncs = uObjectFuncs;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            _logger.Debug("Starting Pattern Scan");

            using var bulkScanner = new BulkScanner(_patternResolver);
            bulkScanner
                .ResolveMachineCodeProperty(_enginePattern, nameof(_enginePattern.PGUObjectArray))
                .ResolveMachineCodeProperty(_enginePattern, nameof(_enginePattern.PNamePoolData))

                .ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetExternalPackage))
                .ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetParentPackage));
            //.ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetFullName));

            bulkScanner.ScanAllProperties();
        }
    }
}
