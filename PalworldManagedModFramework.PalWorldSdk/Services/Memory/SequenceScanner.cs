using System.Diagnostics;

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

            var sw = Stopwatch.StartNew();

            foreach (var scanRegion in memoryRegions) {
                var foundAddresses = ScanMemoryRegion(signatures, scanRegion);
                for (var x = 0; x < foundPatterns.Length; x++) {
                    foundPatterns[x].AddRange(foundAddresses[x]);
                }
            }

            sw.Stop();
            _logger.Debug($"Scanning took {sw.ElapsedMilliseconds} ms.");

            return foundPatterns.Select(i => i.ToArray()).ToArray();
        }

        public unsafe nint[][] ScanMemoryRegion(string[] signatures, MemoryRegion memoryRegion) {
            var foundSequencesPerPattern = new List<nint>[signatures.Length];
            for (var x = 0; x < foundSequencesPerPattern.Length; x++) {
                foundSequencesPerPattern[x] = new List<nint>();
            }

            var patternMasks = signatures.Select(PatternResolver.DeriveMask).ToArray();
            var patternPtrs = new int[patternMasks.Length];

            var scanPtr = (byte*)memoryRegion.StartAddress;
            var endPtr = (byte*)memoryRegion.EndAddress;

            while (scanPtr < endPtr) {
                for (var x = 0; x < patternMasks.Length; x++) {
                    var mask = patternMasks[x].Mask;
                    var pattern = patternMasks[x].Pattern;

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

            return foundSequencesPerPattern.Select(i => i.ToArray()).ToArray();
        }
    }
}
