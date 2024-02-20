using System.Runtime.InteropServices;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.AssemblerServices.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;
using PalworldManagedModFramework.Sdk.Services.Memory;

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

        public unsafe DetourRecord PrepareDetour(nint detourAddress, nint redirect) {
            var detourCodes = _shellCodeFactory.BuildStackDetour64(redirect);

            var minPatchSize = _shellCodeReader.FindMinPatchSize(detourAddress, detourCodes.Length, 64);
            var originalCodes = new ReadOnlySpan<byte>((byte*)detourAddress, minPatchSize).ToArray();

            var reEntryAddress = detourAddress + detourCodes.Length;
            var trampoline = AllocateTrampoline(originalCodes, reEntryAddress);

            return new DetourRecord(detourCodes, originalCodes, detourAddress, redirect, trampoline, DetourType.Stack);
        }

        private unsafe nint AllocateTrampoline(byte[] originalCodes, nint reEntryAddress) {
            var trampolineCodes = _shellCodeFactory.BuildTrampoline64(originalCodes, reEntryAddress);
            var trampoline = _memoryAllocate.Allocate(MemoryProtection.ReadWrite, (uint)trampolineCodes.Length);

            Marshal.Copy(trampolineCodes, 0, trampoline, trampolineCodes.Length);

            _memoryAllocate.SetProtection(trampoline, (uint)trampolineCodes.Length, MemoryProtection.Execute, out _);
            return trampoline;
        }
    }
}
