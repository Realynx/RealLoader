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

        public nint[][] ScanMemoryRegions(ByteCodePattern[] patterns, IEnumerable<MemoryRegion> memoryRegions) {
            var foundPatternsOrdred = new List<nint>[patterns.Length];
            InitLists(foundPatternsOrdred);

            foreach (var scanRegion in memoryRegions) {
                var foundAddresses = ScanMemoryRegionImpl(patterns, scanRegion);
                AppendFoundAddresses(foundPatternsOrdred, foundAddresses);
            }

            return foundPatternsOrdred.Select(i => i.ToArray()).ToArray();
        }

        public nint[][] ScanMemoryRegion(ByteCodePattern[] patterns, MemoryRegion memoryRegion) {
            return ScanMemoryRegionImpl(patterns, memoryRegion).Select(i => i.ToArray()).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private unsafe List<nint>[] ScanMemoryRegionImpl(ByteCodePattern[] patterns, MemoryRegion memoryRegion) {
            var foundSequencesPerPattern = new List<nint>[patterns.Length];
            for (var x = 0; x < foundSequencesPerPattern.Length; x++) {
                foundSequencesPerPattern[x] = new List<nint>();
            }

            var patternPtrs = new int[patterns.Length];

            var scanPtr = (byte*)memoryRegion.StartAddress;
            var endPtr = (byte*)memoryRegion.EndAddress;

            while (scanPtr < endPtr) {
                var scannedByte = *scanPtr;

                for (var x = 0; x < patterns.Length; x++) {
                    var mask = patterns[x].Mask;
                    var pattern = patterns[x].Pattern;

                    ref var patternPtr = ref patternPtrs[x];
                    if (mask[patternPtr] == '?' || pattern[patternPtr] == scannedByte) {
                        patternPtr++;
                        if (patternPtr == pattern.Length) {
                            var foundAddress = (nint)(scanPtr - pattern.Length + 1);
                            _logger.Debug($"[0x{foundAddress:X}] Found: {string.Join(" ", patterns[x].Pattern.Select(i => i.ToString("X")))}");

                            foundSequencesPerPattern[x].Add(foundAddress + patterns[x].OperandOffset);
                            patternPtr = 0;
                        }
                    }
                    else {
                        patternPtr = 0;
                    }
                }

                scanPtr++;
            }

            return foundSequencesPerPattern;
        }

        private static void InitLists(List<nint>[] foundPatternsOrdred) {
            for (var x = 0; x < foundPatternsOrdred.Length; x++) {
                foundPatternsOrdred[x] = new List<nint>();
            }
        }

        private static void AppendFoundAddresses(List<nint>[] foundPatternsOrdred, List<nint>[] foundAddresses) {
            for (var x = 0; x < foundPatternsOrdred.Length; x++) {
                foundPatternsOrdred[x].AddRange(foundAddresses[x]);
            }
        }
    }
}
