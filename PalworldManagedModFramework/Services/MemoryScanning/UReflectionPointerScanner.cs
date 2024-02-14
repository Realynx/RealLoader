using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Services.MemoryScanning.Interfaces;
using PalworldManagedModFramework.UnrealSdk.Services.Data.CoreUObject.FunctionServices;

namespace PalworldManagedModFramework.Services.MemoryScanning {
    internal class UReflectionPointerScanner {
        private readonly ILogger _logger;
        private readonly IEnginePattern _enginePattern;
        private readonly IPatternResolver _patternResolver;
        private readonly IUObjectFuncs _uObjectFuncs;

        public UReflectionPointerScanner(ILogger logger, IEnginePattern enginePattern, IPatternResolver patternResolver, IUObjectFuncs uObjectFuncs) {
            _logger = logger;
            _enginePattern = enginePattern;
            _patternResolver = patternResolver;
            _uObjectFuncs = uObjectFuncs;
        }

        public void ScanMemoryForUnrealReflectionPointers() {
            // TODO: Make the scanner do one scan for each pattern instead of multiple scans. 
            // probably use some sort of IDisposable instance for scan instance
            _logger.Debug("Starting Pattern Scan");

            ResolveMachineCodeProperty(_enginePattern, nameof(_enginePattern.PGUObjectArray));
            ResolveMachineCodeProperty(_enginePattern, nameof(_enginePattern.PNamePoolData));

            // Functions
            ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetExternalPackage));
            ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetParentPackage));
            //ResolveMachineCodeProperty(_uObjectFuncs, nameof(_uObjectFuncs.GetFullName));

        }

        private void ResolveMachineCodeProperty(object instance, string propertyToResolve) {
            var propInfo = instance.GetType().GetProperty(propertyToResolve);
            var resolvedAddress = _patternResolver.ResolvePattern(propInfo, instance);
            _logger.Debug($"Resolved {propertyToResolve}: 0x{resolvedAddress:X}");
        }
    }
}
