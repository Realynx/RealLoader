using System.Diagnostics;
using System.Numerics;

using RealLoaderFramework.Sdk.Logging;
using RealLoaderFramework.Sdk.Services.Detour.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Interfaces;
using RealLoaderFramework.Sdk.Services.Memory.Models;

namespace RealLoaderFramework.Sdk.Services.Detour {
    public class InstructionPatcher : IInstructionPatcher {
        private readonly ILogger _logger;
        private readonly IMemoryAllocate _memoryAllocate;

        public InstructionPatcher(ILogger logger, IMemoryAllocate memoryAllocate) {
            _logger = logger;
            _memoryAllocate = memoryAllocate;
        }

        public unsafe byte[] PatchLiveInstructions(nint address, ReadOnlySpan<byte> instructionBytes) {
            var length = instructionBytes.Length;

            var destinationInstructions = new Span<byte>((byte*)address, length);
            var overwrittenInstructions = new byte[length];

            const SimpleMemoryProtection RWE = SimpleMemoryProtection.Read | SimpleMemoryProtection.Write | SimpleMemoryProtection.Execute;
            _memoryAllocate.SetProtection(address, (uint)instructionBytes.Length, RWE, out var previousProtection);

            var i = 0;

            if (Vector.IsHardwareAccelerated) {
                var vectorSize = Vector<byte>.Count;
                for (; i <= length - vectorSize; i += vectorSize) {
                    var destinationOffset = destinationInstructions[i..];
                    var instructionBytesOffset = instructionBytes[i..];

                    new Vector<byte>(destinationOffset).CopyTo(overwrittenInstructions, i);
                    new Vector<byte>(instructionBytesOffset).CopyTo(destinationOffset);
                }
            }

            // TODO: Add ulong copy method before resorting to individual bytes

            for (; i < length; i++) {
                overwrittenInstructions[i] = destinationInstructions[i];
                destinationInstructions[i] = instructionBytes[i];
            }

            _memoryAllocate.SetProtection(address, (uint)instructionBytes.Length, previousProtection, out var writtenProtection);
            Debug.Assert(writtenProtection == RWE);

            return overwrittenInstructions;
        }
    }
}
