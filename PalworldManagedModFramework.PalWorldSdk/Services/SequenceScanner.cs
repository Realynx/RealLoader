using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using PalworldManagedModFramework.PalWorldSdk.Services.Interfaces;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory;
using PalworldManagedModFramework.PalWorldSdk.Services.Memory.Models;

namespace PalworldManagedModFramework.PalWorldSdk.Services {
    // TODO: Make this Singleton pattern using instance, Ex: Mkaes this unit-testable and interchangble.
    public class SequenceScanner : ISequenceScanner {
        private static readonly Regex _hexRegex = new(@"[0-9a-fA-F]{2}");

        private readonly IMemoryMapper _memoryMapper;
        private readonly IProcessSuspender _processSuspender;

        public SequenceScanner(IMemoryMapper memoryMapper, IProcessSuspender processSuspender) {
            _memoryMapper = memoryMapper;
            _processSuspender = processSuspender;
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
            var startAddress = processModule.BaseAddress;
            var endAddress = processModule.BaseAddress + processModule.ModuleMemorySize;
            return SequenceScan(signature, startAddress, endAddress);
        }

        public nint[] SequenceScan(string signature, nint startAddress, nint endAddress) {
            _processSuspender.PauseSelf();

            var memoryRegions = _memoryMapper.FindMemoryRegions()
                .Where(i => i.ReadFlag)
                .Where(i => (startAddress <= (nint)i.EndAddress && endAddress >= (nint)i.StartAddress));

            var foundSequences = ScanMemoryRegions(signature, memoryRegions);

            _processSuspender.ResumeSelf();
            return foundSequences.ToArray();
        }

        private List<nint> ScanMemoryRegions(string signature, IEnumerable<MemoryRegion> memoryRegions) {
            var foundSequences = new List<nint>();

            foreach (var scanRegion in memoryRegions) {
                var foundAddresses = ScanMemoryRegion(signature, scanRegion);
                if (foundAddresses.Length > 0) {
                    foundSequences.AddRange(foundAddresses);
                }
            }

            return foundSequences;
        }

        private unsafe nint[] ScanMemoryRegion(string signature, MemoryRegion memoryRegion) {
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
