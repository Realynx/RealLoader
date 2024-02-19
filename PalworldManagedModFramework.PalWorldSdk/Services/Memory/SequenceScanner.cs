using System.Runtime.CompilerServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Memory {
    public class SequenceScanner : ISequenceScanner {
        private readonly ILogger _logger;

        public SequenceScanner(ILogger logger) {
            _logger = logger;
        }

        public nint[][] ScanMemoryRegions(string[] signatures, IEnumerable<MemoryRegion> memoryRegions) {
            var foundPatterns = new List<nint>[signatures.Length];
            for (var x = 0; x < foundPatterns.Length; x++) {
                foundPatterns[x] = new List<nint>();
            }

            var patterns = signatures.Select(PatternResolver.DeriveMask).ToArray();

            foreach (var scanRegion in memoryRegions) {
                var foundAddresses = ScanMemoryRegionImpl(patterns, scanRegion);
                for (var x = 0; x < foundPatterns.Length; x++) {
                    foundPatterns[x].AddRange(foundAddresses[x]);
                }
            }

            return foundPatterns.Select(i => i.ToArray()).ToArray();
        }

        public nint[][] ScanMemoryRegion(string[] signatures, MemoryRegion memoryRegion) {
            var patterns = signatures.Select(PatternResolver.DeriveMask).ToArray();

            return ScanMemoryRegionImpl(patterns, memoryRegion).Select(i => i.ToArray()).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private unsafe List<nint>[] ScanMemoryRegionImpl(ByteCodePattern[] patterns, MemoryRegion memoryRegion) {
            var foundSequencesPerPattern = new List<nint>[patterns.Length];
            for (var x = 0; x < foundSequencesPerPattern.Length; x++) {
                foundSequencesPerPattern[x] = new List<nint>();
            }

            var patternPtrs = new int[patterns.Length];

            var scanPtr = (byte*)memoryRegion.StartAddress;
            var endPtr = (byte*)memoryRegion.EndAddress;

            while (scanPtr < endPtr) {
                for (var x = 0; x < patterns.Length; x++) {
                    var mask = patterns[x].Mask;
                    var pattern = patterns[x].Pattern;

                    var patternPtr = patternPtrs[x];
                    if (mask[patternPtr] == '?' || pattern[patternPtr] == *scanPtr) {
                        patternPtrs[x]++;
                        if (patternPtrs[x] == pattern.Length) {
                            foundSequencesPerPattern[x].Add((nint)(scanPtr - pattern.Length + 1));
                            patternPtrs[x] = 0;
                        }
                    }
                    else {
                        patternPtrs[x] = 0;
                    }
                }

                scanPtr++;
            }

            return foundSequencesPerPattern;
        }
    }
}
