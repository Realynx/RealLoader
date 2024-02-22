using System.Numerics;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public class InstructionPatcher : IInstructionPatcher {
        private readonly ILogger _logger;
        private readonly IMemoryAllocate _memoryAllocate;

        public InstructionPatcher(ILogger logger, IMemoryAllocate memoryAllocate) {
            _logger = logger;
            _memoryAllocate = memoryAllocate;
        }

        public unsafe byte[] PatchInstructions(nint address, ReadOnlySpan<byte> instructionBytes) {
            var length = instructionBytes.Length;

            var destinationInstructions = new Span<byte>((byte*)address, length);
            var overwrittenInstructions = new byte[length];

            _memoryAllocate.SetProtection(address, (uint)instructionBytes.Length, SimpleMemoryProtection.Read | SimpleMemoryProtection.Write | SimpleMemoryProtection.Execute, out var previousProtection);

            var i = 0;

            // TODO: Use ulong instead of Vector
            var vectorSize = Vector<byte>.Count;
            for (; i <= length - vectorSize; i += vectorSize) {
                var destinationOffset = destinationInstructions.Slice(i);
                var instructionBytesOffset = instructionBytes.Slice(i);

                new Vector<byte>(destinationOffset).CopyTo(overwrittenInstructions, i);
                new Vector<byte>(instructionBytesOffset).CopyTo(destinationOffset);
            }

            for (; i < length; i++) {
                overwrittenInstructions[i] = destinationInstructions[i];
                destinationInstructions[i] = instructionBytes[i];
            }

            _memoryAllocate.SetProtection(address, (uint)instructionBytes.Length, previousProtection, out _);

            return overwrittenInstructions;
        }
    }
}
