using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PalworldManagedModFramework.PalWorldSdk.Services {
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
        /// Returns the memory positions of a found signature.
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="processModule"></param>
        /// <param name="findCount"></param>
        /// <returns></returns>
        public static IntPtr[] SequenceScan(string signature, ProcessModule processModule, int findCount = 1) {
            var patternMask = DeriveMask(signature);
            var memoryLocation = (byte*)processModule.BaseAddress.ToPointer();

            var foundSequences = new List<IntPtr>();
            var checkBuffer = new byte[patternMask.pattern.Length];
            var statAddress = (uint)memoryLocation;

            try {

                for (var x = statAddress; x < statAddress + processModule.ModuleMemorySize; x++) {
                    if ((*memoryLocation) != patternMask.pattern[0]) {
                        memoryLocation += 1;
                        continue;
                    }

                    for (var y = 0; y < checkBuffer.Length; y++) {
                        if (patternMask.mask[y] != 'x') {
                            checkBuffer[y] = 0x00;
                        }
                        else {
                            checkBuffer[y] = *memoryLocation;
                        }
                        memoryLocation += 1;
                    }

                    if (checkBuffer.SequenceEqual(patternMask.pattern)) {
                        foundSequences.Add(new IntPtr(memoryLocation - patternMask.pattern.Length));
                        if (foundSequences.Count >= findCount) {
                            return foundSequences.ToArray();
                        }
                    }
                }
            }
            catch {
                return foundSequences.ToArray();
            }

            return foundSequences.ToArray();
        }

        private static (char[] mask, byte[] pattern) DeriveMask(string signature) {
            var hexValues = signature.Split(' ');

            if (hexValues.Length < 2) {
                throw new Exception($"Invalid pattern, your {nameof(signature)} is not long enough.");
            }

            if (!_hexRegex.IsMatch(hexValues[0])) {
                throw new Exception("Invalid pattern, you may not start a pattern with a wild card.");
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
