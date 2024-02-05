using System.Globalization;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.PalWorldSdk.Logging;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Models;

namespace PalworldManagedModFramework.PalWorldSdk.Services.Memory {
    public class SequenceScanner : ISequenceScanner {
        private static readonly Regex _hexRegex = new(@"[0-9a-fA-F]{2}");
        private readonly ILogger _logger;

        public SequenceScanner(ILogger logger) {
            _logger = logger;
        }

        /// <summary>
        /// Scan memory regions and return the found addresses for your pattern.
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="memoryRegions"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This is an unsafe function that will scan the provided memory region. Be very careful using this!
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="memoryRegion"></param>
        /// <returns></returns>
        public unsafe nint[] ScanMemoryRegion(string signature, MemoryRegion memoryRegion) {
            var foundSequences = new List<nint>();

            var scanPtr = (nint)memoryRegion.StartAddress;
            var endPtr = (nint)memoryRegion.EndAddress;
            var patternPtr = 0;

            var patternMask = DeriveMask(signature);
            var patternLength = patternMask.pattern.Length;

            while (scanPtr < endPtr) {
                var scanByte = ((byte*)scanPtr)[patternPtr];

                var matchedValue = patternMask.mask[patternPtr] == '?' || patternMask.pattern[patternPtr] == scanByte;
                if (matchedValue) {
                    patternPtr++;

                    if (patternPtr == patternLength) {
                        foundSequences.Add(scanPtr + patternMask.offset);
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
            var offset = -1;

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
