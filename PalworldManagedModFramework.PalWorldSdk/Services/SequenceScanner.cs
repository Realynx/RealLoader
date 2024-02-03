using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.PalWorldSdk.Services.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory;

namespace PalworldManagedModFramework.PalWorldSdk.Services {
    // TODO: Make this Singleton pattern using instance, Ex: Mkaes this unit-testable and interchangble.
    public class SequenceScanner : ISequenceScanner {
        private static readonly Regex _hexRegex = new(@"[0-9a-fA-F]{2}");

        private readonly IMemoryMapper _memoryMapper;

        public SequenceScanner(IMemoryMapper memoryMapper) {
            _memoryMapper = memoryMapper;
        }

        /// <summary>
        /// Returns the memory position of a found signature, from the main process module.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public IntPtr? SingleSequenceScan(string signature) {
            return SequenceScan(signature, Process.GetCurrentProcess().MainModule
                ?? throw new Exception("Main module was null")).FirstOrDefault();
        }

        /// <summary>
        /// Returns the memory positions of a found signature, from the main process module.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public IntPtr[] SequenceScan(string signature) {
            return SequenceScan(signature, Process.GetCurrentProcess().MainModule
                ?? throw new Exception("Main module was null"));
        }

        /// <summary>
        /// Scans a process module's memory for a specified signature pattern with wildcards using a two-pointer algorithm for efficient matching.
        /// </summary>
        /// <param name="signature">The signature pattern to search for with '??' as wildcards.</param>
        /// <param name="processModule">The process module to scan.</param>
        /// <param name="findCount">The number of matches to find before stopping the scan. Default is 1.</param>
        /// <returns>An array of memory addresses where the signature was found.</returns>
        public nint[] SequenceScan(string signature, ProcessModule processModule) {
            var memoryRegions = _memoryMapper.FindMemoryRegions(processModule).Where(i => i.ReadFlag);
            var foundSequences = new List<nint>();

            var loopResult = Parallel.ForEach(memoryRegions, (scanRegion) => {
                var foundAddresses = ScanMemoryRegion(signature, scanRegion);
                if (foundAddresses.Length < 1) {
                    return;
                }

                lock (foundSequences) {
                    foundSequences.AddRange(foundAddresses);
                }
            });


            while (!loopResult.IsCompleted) {
                Thread.Sleep(100);
            }

            return foundSequences.ToArray();
        }

        private unsafe nint[] ScanMemoryRegion(string signature, MemoryRegion memoryRegion) {
            var patternMask = DeriveMask(signature);
            var patternLength = patternMask.pattern.Length;

            var foundSequences = new List<nint>();

            var scanPtr = (nint)memoryRegion.StartAddress;
            var endPtr = (nint)memoryRegion.EndAddress;
            var patternPtr = 0;

            while (scanPtr < endPtr) {
                var matchedValue = patternMask.mask[patternPtr] == '?' || patternMask.pattern[patternPtr] == ((byte*)scanPtr)[patternPtr];
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


        private (char[] mask, byte[] pattern, int offset) DeriveMask(string signature) {
            var hexValues = signature.Split(' ');

            if (hexValues.Length < 2) {
                throw new Exception($"Invalid pattern, your {nameof(signature)} is not long enough.");
            }

            var mask = new List<char>();
            var buffer = new List<byte>();
            var offset = 0;

            for (var x = 0; x < hexValues.Length; x++) {
                if (_hexRegex.IsMatch(hexValues[x])) {
                    buffer.Add(byte.Parse(hexValues[x], NumberStyles.HexNumber));
                    mask.Add('x');
                }
                else if (hexValues[x] == "?") {
                    buffer.Add(0x00);
                    mask.Add('?');
                }
                else if (hexValues[x] == "|") {
                    if (offset != -1) {
                        throw new Exception("Multiple offset indicators '|' found in the pattern.");
                    }

                    offset = x;
                }
            }

            if (buffer.Count != mask.Count) {
                throw new Exception("Invalid Signature");
            }

            offset = offset == -1 ? 0 : offset;
            return (mask.ToArray(), buffer.ToArray(), offset);
        }
    }
}
