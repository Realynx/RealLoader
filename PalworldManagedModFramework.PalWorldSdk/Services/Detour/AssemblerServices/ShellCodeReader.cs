using System.Net;

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
            var instructions = DisAssembleCodes(address, patchLength, bitness);

            var minPatchSize = patchLength;
            foreach (var instruction in instructions) {
                if (instruction.IP < (ulong)(address + patchLength)) {
                    continue;
                }

                var instructionEnd = instruction.IP + (ulong)instruction.Length;
                if (instructionEnd > (ulong)(address + patchLength)) {
                    minPatchSize = (int)(instructionEnd - (ulong)address);
                    break;
                }
            }

            return minPatchSize;
        }

        public unsafe byte[] FixRelativeOffsets(nint originalAddress, nint newAddress, byte[] codes, int bitness) {
            var instructions = DisAssembleCodes(originalAddress, codes, bitness);
            var offsetChange = newAddress - originalAddress;

            for (var x = 0; x < instructions.Count; x++) {
                var instruction = instructions[x];

                instruction.IP += (ulong)offsetChange;
                if (instruction.IsIPRelativeMemoryOperand) {
                    var newOffset = instruction.MemoryDisplacement32 + offsetChange;

                    if (newOffset is > int.MaxValue or < int.MinValue) {
                        throw new InvalidOperationException("Offset change is too large for a 32-bit displacement.");
                    }

                    instruction.MemoryDisplacement32 = (uint)newOffset;
                    instruction.NearBranch64 += (ulong)offsetChange;
                }
            }

            return AssembleInstructions(instructions, newAddress, bitness);
        }

        private byte[] AssembleInstructions(List<Instruction> instructions, nint newAddress, int bitness) {
            var codeWriter = new CodeWriterImpl();
            var encoder = Encoder.Create(bitness, codeWriter);

            foreach (var instruction in instructions) {
                encoder.Encode(instruction, (ulong)newAddress);
                newAddress += instruction.Length;
            }

            return codeWriter.ToArray();
        }

        private static unsafe List<Instruction> DisAssembleCodes(nint address, int patchLength, int bitness) {
            var length = patchLength + 64;

            var codes = new ReadOnlySpan<byte>((byte*)address, length).ToArray();
            var instructions = DisAssembleCodes(address, codes, bitness);

            return instructions;
        }

        private static unsafe List<Instruction> DisAssembleCodes(nint address, byte[] codes, int bitness) {
            var codeReader = new ByteArrayCodeReader(codes);
            var decoder = Decoder.Create(bitness, codeReader);

            decoder.IP = (ulong)address;
            var endIp = decoder.IP + (ulong)codes.Length;

            var instructions = new List<Instruction>();
            while (decoder.IP < endIp) {
                instructions.Add(decoder.Decode());
            }

            return instructions;
        }
    }
}
