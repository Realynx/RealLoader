using Iced.Intel;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;

namespace PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices {
    public class ShellCodeReader : IShellCodeReader {
        private readonly ILogger _logger;

        public ShellCodeReader(ILogger logger) {
            _logger = logger;
        }

        public unsafe int FindMinPatchSize(nint address, int patchLength, int bitness) {
            var length = patchLength + 64;

            var codes = new ReadOnlySpan<byte>((byte*)address, length).ToArray();

            var codeReader = new ByteArrayCodeReader(codes);
            var decoder = Decoder.Create(bitness, codeReader);

            decoder.IP = (ulong)address;
            var endIp = decoder.IP + (ulong)length;

            var instructions = new List<Instruction>();
            while (decoder.IP < endIp) {
                instructions.Add(decoder.Decode());
            }

            var minPatchSize = patchLength;
            foreach (var instruction in instructions) {
                if (instruction.IP >= (ulong)(address + patchLength)) {
                    var instructionEnd = instruction.IP + (ulong)instruction.Length;

                    if (instructionEnd > (ulong)(address + patchLength)) {
                        minPatchSize = (int)(instructionEnd - (ulong)address);
                        break;
                    }
                }
            }

            return minPatchSize;
        }
    }
}
