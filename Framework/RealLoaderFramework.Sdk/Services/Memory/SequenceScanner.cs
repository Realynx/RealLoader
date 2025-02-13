using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Memory {
    public class SequenceScanner : ISequenceScanner {
        private readonly ILogger _logger;

        public SequenceScanner(ILogger logger) {
            _logger = logger;
        }

        public nint[][] ScanMemoryRegions(ByteCodePattern[] patterns, IEnumerable<MemoryRegion> memoryRegions) {
            var foundPatternsOrdered = new List<nint>[patterns.Length];
            InitLists(foundPatternsOrdered);

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(memoryRegions, parallelOptions, scanRegion => {
                var foundAddresses = ScanMemoryRegionImpl(patterns, scanRegion);
                AppendFoundAddresses(foundPatternsOrdered, foundAddresses);
            });

            return foundPatternsOrdered
                .Select(i => i.ToArray())
                .ToArray();
        }

        public nint[][] ScanMemoryRegion(ByteCodePattern[] patterns, MemoryRegion memoryRegion) {
            return ScanMemoryRegionImpl(patterns, memoryRegion)
                .Select(i => i.ToArray())
                .ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private unsafe List<nint>[] ScanMemoryRegionImpl(ByteCodePattern[] patterns, MemoryRegion memoryRegion) {
            var foundSequencesPerPattern = new List<nint>[patterns.Length];
            InitLists(foundSequencesPerPattern);

            // Somehow this is 2-4% faster as an array than a stackalloc (span or pointer)
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
                            var foundAddress = (nint)(scanPtr - pattern.Length + 1) + patterns[x].OperandOffset;
                            foundSequencesPerPattern[x].Add(foundAddress);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InitLists(List<nint>[] listArray) {
            for (var x = 0; x < listArray.Length; x++) {
                listArray[x] = new List<nint>();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Instanced method allows MethodImplOptions.Synchronized to lock on the instance rather than the type.")]
        private void AppendFoundAddresses(List<nint>[] foundPatternsOrdered, List<nint>[] foundAddresses) {
            for (var x = 0; x < foundPatternsOrdered.Length; x++) {
                foundPatternsOrdered[x].AddRange(foundAddresses[x]);
            }
        }
    }
}
