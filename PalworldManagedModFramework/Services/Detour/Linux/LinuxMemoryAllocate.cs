﻿using Microsoft.Extensions.Logging;

using PalworldManagedModFramework.Services.Detour.Models;

using PalworldManagedModFramework.Services.Detour.Interfaces;

using static PalworldManagedModFramework.Services.Detour.Linux.NativeFunctions;


namespace PalworldManagedModFramework.Services.Detour.Linux {
    public class LinuxMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;

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
                _logger.LogInformation($"Memory allocated. Address: 0x{allocatedMemory:x}, Length: {length}, Protection: {linuxProtection}");
                return allocatedMemory;
            }
        }

        public bool Free(nint address, nuint length) {
            var result = MemoryUnmap(address, length);

            if (result == -1) {
                _logger.LogError($"Failed to free memory at address: {address}");
                return false;
            }
            else if (result == 0) {
                _logger.LogInformation($"Memory freed at address: {address}");
                return true;
            }
            else {
                _logger.LogWarning($"Unexpected result from munmap: {result}. Assuming success.");
                return true;
            }
        }
    }
}
