using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

using PalworldManagedModFramework.Services.Detour.Models;

using PalworldManagedModFramework.Services.Detour.Interfaces;

using static PalworldManagedModFramework.Services.Detour.Linux.NativeFunctions;


namespace PalworldManagedModFramework.Services.Detour.Linux {
    public class LinuxMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<nint, nuint> _mappedAddresses = new();

        public LinuxMemoryAllocate(ILogger logger) {
            _logger = logger;
        }

        public nint Allocate(MemoryProtection protection, uint length) {
            var linuxProtection = protection switch {
                MemoryProtection.Execute => MProtectProtect.PROT_EXEC | MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE,
                MemoryProtection.ReadWrite => MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE,
                MemoryProtection.Readonly => MProtectProtect.PROT_READ,
                _ => throw new ArgumentOutOfRangeException(nameof(protection), protection, "Invalid memory protection type.")
            };

            var allocatedMemory = MemoryMap(nint.Zero, length, linuxProtection, MMapFlags.MAP_SHARED | MMapFlags.MAP_ANONYMOUS, -1, 0);

            if (allocatedMemory == -1) {
                _logger.LogError($"Failed to allocate memory. Length: {length}, Protection: {linuxProtection}");
                return nint.Zero;
            }
            else {
                if (!_mappedAddresses.TryAdd(allocatedMemory, length)) {
                    throw new UnreachableException($"Failed to record 0x{allocatedMemory:X} as a mapped address. Already exists: {_mappedAddresses.ContainsKey(allocatedMemory)}.");
                }

                _logger.LogInformation($"Memory allocated. Address: 0x{allocatedMemory:X}, Length: {length}, Protection: {linuxProtection}");
                return allocatedMemory;
            }
        }

        public bool Free(nint address) {
            if (!_mappedAddresses.TryRemove(address, out var length)) {
                _logger.LogError($"Tried to free memory that was not mapped by us. Address: 0x{address:X}.");
                return false;
            }

            var result = MemoryUnmap(address, length);

            if (result == -1) {
                _logger.LogError($"Failed to free memory at address: 0x{address:X}");
                return false;
            }
            else if (result == 0) {
                _logger.LogInformation($"Memory freed at address: 0x{address:X}");
                return true;
            }
            else {
                _logger.LogWarning($"Unexpected result from munmap when freeing 0x{address:X}: {result}. Assuming success.");
                return true;
            }
        }
    }
}
