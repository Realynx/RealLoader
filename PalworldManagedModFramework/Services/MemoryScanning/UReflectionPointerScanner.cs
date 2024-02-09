using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class UReflectionPointerScanner {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IPatternResolver _patternResolver;

        public UReflectionPointerScanner(ILogger logger, IEnginePattern enginePattern, IPatternResolver patternResolver) {
            _logger = logger;
            _enginePattern = enginePattern;
            _patternResolver = patternResolver;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            // TODO: Make the scanner do one scan for each pattern instead of multiple scans. 
            // probably use some sort of IDisposable instance for scan instance
            _logger.Debug("Starting Pattern Scan");

            var objectArrayPropInfo = _enginePattern.GetType().GetProperty(nameof(_enginePattern.PGUObjectArray));
            var objectArrayResolvedAddress = _patternResolver.ResolvePattern(objectArrayPropInfo, _enginePattern);
            _logger.Debug($"Resolved {nameof(_enginePattern.PGUObjectArray)}: 0x{objectArrayResolvedAddress:X}");

            var namePoolDataPropInfo = _enginePattern.GetType().GetProperty(nameof(_enginePattern.PNamePoolData));
            var namePoolDataResolvedAddress = _patternResolver.ResolvePattern(namePoolDataPropInfo, _enginePattern);
            _logger.Debug($"Resolved {nameof(_enginePattern.PNamePoolData)}: 0x{namePoolDataResolvedAddress:X}");
        }
    }
}
