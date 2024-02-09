using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Models;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public class SequenceScanner : ISequenceScanner {
        private readonly ILogger _logger;

        public SequenceScanner(ILogger logger) {
            _logger = logger;
        }

        public IEnumerable<nint> ScanMemoryRegions(string signature, IEnumerable<MemoryRegion> memoryRegions) {
            foreach (var scanRegion in memoryRegions) {
                var foundAddresses = ScanMemoryRegion(signature, scanRegion);

                if (foundAddresses.Length > 0) {
                    foreach (var address in foundAddresses) {
                        yield return address;
                    }
                }
            }
        }

        public unsafe nint[] ScanMemoryRegion(string signature, MemoryRegion memoryRegion) {
            var foundSequences = new List<nint>();

            var scanPtr = (nint)memoryRegion.StartAddress;
            var endPtr = (nint)memoryRegion.EndAddress;
            var patternPtr = 0;

            var patternMask = PatternResolver.DeriveMask(signature);
            var patternLength = patternMask.pattern.Length;

            while (scanPtr < endPtr) {
                var scanByte = ((byte*)scanPtr)[patternPtr];

                var matchedValue = patternMask.mask[patternPtr] == '?' || patternMask.pattern[patternPtr] == scanByte;
                if (matchedValue) {
                    patternPtr++;

                    if (patternPtr == patternLength) {
                        foundSequences.Add(scanPtr);
                        scanPtr++;
                        patternPtr = 0;
                    }
                }
                else {
                    scanPtr += Math.Max(1, patternPtr);
                    patternPtr = 0;
                }
            }

            return foundSequences.ToArray();
        }
    }
}
