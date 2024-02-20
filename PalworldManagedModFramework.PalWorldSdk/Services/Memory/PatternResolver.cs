using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Linux;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public partial class PatternResolver : IPatternResolver {


        private readonly ILogger _logger;
        private readonly IOperandResolver _operandResolver;
        private readonly IMemoryScanner _memoryScanner;

        public PatternResolver(ILogger logger, IOperandResolver operandResolver, IMemoryScanner memoryScanner) {
            _logger = logger;
            _operandResolver = operandResolver;
            _memoryScanner = memoryScanner;
        }

        public nint?[] ResolvePatterns(ByteCodePattern[] patterns) {
            var results = new nint?[patterns.Length];

            var stringPatterns = patterns.Select(i => i.Pattern).ToArray();
            var patternScanResults = _memoryScanner.SequenceScan(patterns);
            if (patternScanResults is null) {
                return Array.Empty<nint?>();
            }

            for (var x = 0; x < patternScanResults.Length; x++) {
                var scanResult = patternScanResults[x];
                var firstMatchedAddress = scanResult.FirstOrDefault();
                if (firstMatchedAddress is 0) {
                    _logger.Error($"Could not resolve pPattern: {patterns[x].Pattern}");
                    results[x] = null;
                }
            }

            return results;
        }

        public nint? ResolvePattern(ByteCodePattern pattern) {
            var patternScanResult = _memoryScanner.SingleSequenceScan(pattern);
            if (patternScanResult is not nint patternOffsetAddress) {
                _logger.Error($"Could not resolve pattern: {pattern.Pattern}");
                return null;
            }

            return patternOffsetAddress;
        }
    }
}