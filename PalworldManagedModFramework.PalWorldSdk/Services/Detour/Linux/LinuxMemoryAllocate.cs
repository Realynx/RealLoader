using System.Collections.Concurrent;
using System.Diagnostics;

using PalworldManagedModFramework.Sdk.Logging;
using PalworldManagedModFramework.Sdk.Services.Detour.Interfaces;
using PalworldManagedModFramework.Sdk.Services.Detour.Models;

using static PalworldManagedModFramework.Sdk.Services.Detour.Linux.NativeFunctions;


namespace PalworldManagedModFramework.Sdk.Services.Detour.Linux {
    public class LinuxMemoryAllocate : IMemoryAllocate {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<nint, nuint> _mappedAddresses = new();

        public LinuxMemoryAllocate(ILogger logger) {
            _logger = logger;
        }

        public nint Allocate(MemoryProtection protection, uint length) {
            var linuxProtection = ConvertToMProtection(protection);

            var allocatedMemory = MemoryMap(nint.Zero, length, linuxProtection, MMapFlags.MAP_SHARED | MMapFlags.MAP_ANONYMOUS, -1, 0);

            if (allocatedMemory == -1) {
                _logger.Error($"Failed to allocate memory. Length: {length}, Protection: {linuxProtection}");
                return nint.Zero;
            }
            else {
                if (!_mappedAddresses.TryAdd(allocatedMemory, length)) {
                    throw new UnreachableException($"Failed to record 0x{allocatedMemory:X} as a mapped address. Already exists: {_mappedAddresses.ContainsKey(allocatedMemory)}.");
                }

                _logger.Info($"Memory allocated. Address: 0x{allocatedMemory:X}, Length: {length}, Protection: {linuxProtection}");
                return allocatedMemory;
            }
        }

        public bool Free(nint address) {
            if (!_mappedAddresses.TryRemove(address, out var length)) {
                _logger.Error($"Tried to free memory that was not mapped by us. Address: 0x{address:X}.");
                return false;
            }

            var result = MemoryUnmap(address, length);

            if (result == -1) {
                _logger.Error($"Failed to free memory at address: 0x{address:X}");
                return false;
            }
            else if (result == 0) {
                _logger.Info($"Memory freed at address: 0x{address:X}");
                return true;
            }
            else {
                _logger.Warning($"Unexpected result from munmap when freeing 0x{address:X}: {result}. Assuming success.");
                return true;
            }
        }

        public bool SetProtection(nint address, uint length, MemoryProtection protection, out MemoryProtection previousProtection) {
            var linuxProtection = ConvertToMProtection(protection);

            var previousProtect = (MProtectProtect)GetMemoryProtect(address);
            var result = MemoryProtect(address, length, linuxProtection);

            if (result == -1) {
                _logger.Error($"Failed to set protection for address: 0x{address:X}");
                previousProtection = MemoryProtection.None;
                return false;
            }
            else if (result == 0) {
                _logger.Info($"Set protection of 0x{address:X} to {protection}.");
                previousProtection = ConvertToMemoryProtection(previousProtect);
                return true;
            }
            else {
                _logger.Warning($"Unexpected result from mmap when setting protection for 0x{address:X}: {result}. Assuming success.");
                previousProtection = ConvertToMemoryProtection(previousProtect);
                return true;
            }
        }

        private MProtectProtect ConvertToMProtection(MemoryProtection memoryProtection) {
            return memoryProtection switch {
                MemoryProtection.Execute => MProtectProtect.PROT_EXEC | MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE,
                MemoryProtection.ReadWrite => MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE,
                MemoryProtection.Readonly => MProtectProtect.PROT_READ,
                _ => throw new ArgumentOutOfRangeException(nameof(memoryProtection), memoryProtection, "Invalid memory protection type.")
            };
        }

        private MemoryProtection ConvertToMemoryProtection(MProtectProtect protection) {
            return protection switch {
                MProtectProtect.PROT_EXEC | MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE  => MemoryProtection.Execute,
                MProtectProtect.PROT_EXEC | MProtectProtect.PROT_READ => MemoryProtection.Execute,
                MProtectProtect.PROT_EXEC | MProtectProtect.PROT_WRITE => MemoryProtection.Execute,
                MProtectProtect.PROT_EXEC => MemoryProtection.Execute,
                MProtectProtect.PROT_READ | MProtectProtect.PROT_WRITE => MemoryProtection.ReadWrite,
                MProtectProtect.PROT_READ => MemoryProtection.Readonly,
                _ => throw new ArgumentOutOfRangeException(nameof(protection), protection, "Invalid memory protection type.")
            };
        }
    }
}
