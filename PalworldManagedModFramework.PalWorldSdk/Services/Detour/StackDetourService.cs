using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.Memory.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Memory.Models;

namespace PalworldManagedModFramework.Sdk.Services.Detour {
    public class StackDetourService : IStackDetourService {
        private readonly ILogger _logger;
        private readonly IShellCodeFactory _shellCodeFactory;
        private readonly IMemoryAllocate _memoryAllocate;
        private readonly IShellCodeReader _shellCodeReader;

        public StackDetourService(ILogger logger, IShellCodeFactory shellCodeFactory, IMemoryAllocate memoryAllocate, IShellCodeReader shellCodeReader) {
            _logger = logger;
            _shellCodeFactory = shellCodeFactory;
            _memoryAllocate = memoryAllocate;
            _shellCodeReader = shellCodeReader;
        }

        public unsafe DetourRecord PrepareDetour(nint detourAddress, nint redirectAddress) {
            var detourCodes = _shellCodeFactory.BuildStackDetour64(redirectAddress);

            var minPatchSize = _shellCodeReader.FindMinPatchSize(detourAddress, detourCodes.Length, 64);
            var originalCodes = new ReadOnlySpan<byte>((byte*)detourAddress, minPatchSize).ToArray();

            var trampoline = AllocateTrampoline(detourAddress, originalCodes);

            return new DetourRecord(detourCodes, originalCodes, detourAddress, redirectAddress, trampoline, DetourType.Stack);
        }

        private unsafe nint AllocateTrampoline(nint originalAddress, byte[] originalCodes) {
            var reEntryAddress = originalAddress + originalCodes.Length;
            var trampolineAddress = _memoryAllocate.Allocate(SimpleMemoryProtection.Read | SimpleMemoryProtection.Write, (uint)originalCodes.Length + 0x10);

            var fixedcodes = _shellCodeReader.FixRelativeOffsets(originalAddress, trampolineAddress, originalCodes, 64);
            var trampolineCodes = _shellCodeFactory.BuildTrampoline64(fixedcodes, reEntryAddress);

            Marshal.Copy(trampolineCodes, 0, trampolineAddress, trampolineCodes.Length);

            _memoryAllocate.SetProtection(trampolineAddress, (uint)trampolineCodes.Length, SimpleMemoryProtection.Read | SimpleMemoryProtection.Execute, out _);
            _logger.Debug("Trampoline memory protected");

            return trampolineAddress;
        }
    }
}
