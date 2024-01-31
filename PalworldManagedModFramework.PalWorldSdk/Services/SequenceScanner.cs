using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PalworldManagedModFramework.PalWorldSdk.Services {
    // TODO: Make this Singleton pattern using instance, Ex: Mkaes this unit-testable and interchangble.
    public static unsafe class SequenceScanner {
        private static readonly Regex _hexRegex = new(@"[0-9a-fA-F]{2}");

        /// <summary>
        /// Returns the memory position of a found signature, from the main process module.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static IntPtr? SingleSequenceScan(string signature) {
            return SequenceScan(signature, Process.GetCurrentProcess().MainModule
                ?? throw new Exception("Main module was null")).FirstOrDefault();
        }

        /// <summary>
        /// Returns the memory positions of a found signature, from the main process module.
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public static IntPtr[] SequenceScan(string signature) {
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
        public static IntPtr[] SequenceScan(string signature, ProcessModule processModule, int findCount = 0) {
            var patternMask = DeriveMask(signature); // Ensure this handles '??' as wildcards.
            var baseAddress = (byte*)processModule.BaseAddress.ToPointer();
            var patternLength = patternMask.pattern.Length;

            var foundSequences = new List<IntPtr>();

            try {
                var scanPtr = baseAddress;
                var endPtr = baseAddress + processModule.ModuleMemorySize - patternLength;
                var patternPtr = 0;

                while (scanPtr < endPtr) {
                    if (patternMask.mask[patternPtr] == '?' || patternMask.pattern[patternPtr] == scanPtr[patternPtr]) {
                        patternPtr++;
                        if (patternPtr == patternLength) {
                            foundSequences.Add(new IntPtr(scanPtr));
                            if (foundSequences.Count >= findCount && findCount > 0) {
                                break;
                            }

                            scanPtr++; // Move scanPtr forward by one byte instead of the pattern length
                            patternPtr = 0; // Reset patternPtr
                        }
                    }
                    else {
                        scanPtr += Math.Max(1, patternPtr); // Move scanPtr forward, skipping checked bytes
                        patternPtr = 0; // Reset patternPtr
                    }
                }

            }
            catch (Exception ex) {
                // Log the exception
                // Example: Console.WriteLine("An error occurred: " + ex.Message);
                return foundSequences.ToArray();
            }

            return foundSequences.ToArray();
        }


        private static (char[] mask, byte[] pattern) DeriveMask(string signature) {
            var hexValues = signature.Split(' ');

            if (hexValues.Length < 2) {
                throw new Exception($"Invalid pattern, your {nameof(signature)} is not long enough.");
            }

            var mask = new string('x', hexValues.Length).ToCharArray();
            var buffer = new byte[hexValues.Length];

            for (var x = 0; x < buffer.Length; x++) {
                if (_hexRegex.IsMatch(hexValues[x])) {
                    buffer[x] = byte.Parse(hexValues[x], NumberStyles.HexNumber);
                }
                else {
                    buffer[x] = 0x00;
                    mask[x] = '?';
                }
            }

            return (mask, buffer);
        }
    }
}
